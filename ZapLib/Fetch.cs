using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ZapLib
{
    public class Fetch
    {
        private MyLog log;
        private List<FileStream> streams = null;
        private string current_data = "";
        private string response;
        private byte[] binaryResponse = null;
        private int statusCode = 0;
        private string proxy = null;

        protected WebRequest req;
        protected HttpClient client;
        protected HttpResponseMessage res;
        protected MultipartFormDataContent formData;

        public string uri { get; set; }
        // set the maxmium binaryResponse size (byte) default 25M
        public int binarySize { get; set; } = 25 * 1024;
        public object header { get; set; } = null;
        public string userAgent { get; set; } = null;
        public string contentType { get; set; } = "application/json";
        public bool isRaw { get; set; } = false;
        public WebResponse webResponse { get; set; } = null;

        public Fetch()
        {
            log = new MyLog();
            proxy = Config.get("Proxy");
        }

        public Fetch(string url)
        {
            uri = url;
            log = new MyLog();
            proxy = Config.get("Proxy");
        }

        private void appendHeaders()
        {

            if (header == null) return;
            if (header.GetType() == typeof(Dictionary<string, string>))
            {
                foreach (KeyValuePair<string, string> prop in (Dictionary<string, string>)header)
                {
                    if (req != null) req.Headers.Add(prop.Key, prop.Key);
                    if (client != null) client.DefaultRequestHeaders.Add(prop.Key, prop.Key);
                }
            }
            else
            {
                try
                {
                    foreach (var prop in header.GetType().GetProperties())
                    {
                        string name = prop.Name;
                        object value = prop.GetValue(header, null);
                        if (value != null)
                        {
                            if (req != null) req.Headers.Add(name, value.ToString());
                            if (client != null) client.DefaultRequestHeaders.Add(name, value.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    log.write("can not set header:" + JsonConvert.SerializeObject(header));
                    log.write("can not set header:" + e.ToString());
                }
            }

            if (req != null && userAgent != null) ((HttpWebRequest)req).UserAgent = userAgent;
            if (client != null && userAgent != null) client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        /*
            call HTTP POST and return response
            (string object) 
        */
        public T post<T>(object data, object query = null, object files = null)
        {
            bool statusCode;
            if (files == null)
            {
                setPost(data, query);
                statusCode = send();
            }
            else
            {
                setUpload(data, files);
                statusCode = upload(query);
            }
            return statusCode ? getResponse<T>() : default(T);
        }

        /*
            call HTTP POST and return response
            (string present) 
        */
        public string post(object data, object query = null, object files = null)
        {
            bool statusCode;
            if (files == null)
            {
                setPost(data, query);
                statusCode = send();
            }
            else
            {
                setUpload(data, files);
                statusCode = upload(query);
            }
            return statusCode ? getResponse() : null;
        }

        /*
            call HTTP GET and return response
            (object present) 
        */
        public T get<T>(object data)
        {
            setGet(data);
            return send() ? getResponse<T>() : default(T);
        }

        /*
            call HTTP GET and return response
            (string present) 
        */
        public string get(object data)
        {
            setGet(data);
            return send() ? getResponse() : null;
        }

        public byte[] getBinary(object data)
        {
            setGet(data);
            return send(true) ? getBinaryResponse() : null;
        }


        /*
            call HTTP DELETE , just like POST 
        */
        public string delete(object data, object query = null)
        {
            setDelete(data, query);
            return send() ? getResponse() : null;
        }

        /*
            call HTTP DELETE , just like POST 
        */
        public T delete<T>(object data, object query = null)
        {
            setDelete(data, query);
            return send() ? getResponse<T>() : default(T);
        }


        /*
            set HTTP POST required info
        */
        private void setPost(object data, object query)
        {
            req = WebRequest.Create(procURI(query));
            req.ContentType = contentType;
            req.Method = "POST";
            appendHeaders();

            if (!string.IsNullOrWhiteSpace(proxy))
                req.Proxy = new WebProxy(proxy, true);

            if (data != null)
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    current_data = isRaw ? data.ToString() : JsonConvert.SerializeObject(data);
                    streamWriter.Write(current_data);
                    streamWriter.Flush();
                }
        }


        /*
            set HTTP DELETE required info
        */
        private void setDelete(object data, object query)
        {
            req = WebRequest.Create(procURI(query));
            req.ContentType = contentType;
            req.Method = "DELETE";
            appendHeaders();

            if (!string.IsNullOrWhiteSpace(proxy))
                req.Proxy = new WebProxy(proxy, true);

            if (data != null)
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    current_data = isRaw ? data.ToString() : JsonConvert.SerializeObject(data);
                    streamWriter.Write(current_data);
                    streamWriter.Flush();
                }
        }

        /*
            set HTTP GET required info
        */
        private void setGet(object data)
        {
            req = WebRequest.Create(procURI(data));
            req.ContentType = contentType;
            req.Method = "GET";
            appendHeaders();

            if (!string.IsNullOrWhiteSpace(proxy))
                req.Proxy = new WebProxy(proxy, true);
        }

        /*
            Append file and text data into Form
            set required info
        */
        public void setUpload(object data, object files)
        {
            HttpClientHandler httpClientHandler = null;

            if (!string.IsNullOrWhiteSpace(proxy))
            {
                httpClientHandler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(proxy, true)
                };
            }
            client = httpClientHandler == null ? new HttpClient() : new HttpClient(httpClientHandler);
            formData = new MultipartFormDataContent();
            appendHeaders();

            // add text data to form
            if (data != null)
            {
                foreach (var prop in data.GetType().GetProperties())
                {
                    if (prop.GetValue(data, null) == null) continue;
                    StringContent content = new StringContent(prop.GetValue(data, null).ToString());
                    formData.Add(content, prop.Name);
                }
            }
            // add file data to form      
            if (files != null)
            {
                streams = new List<FileStream>();
                foreach (var prop in files.GetType().GetProperties())
                {
                    string filePath = prop.GetValue(files, null).ToString();

                    if (!File.Exists(filePath)) continue;

                    FileStream fileStream = File.OpenRead(filePath);
                    streams.Add(fileStream);

                    HttpContent fileStreamContent = new StreamContent(fileStream);
                    formData.Add(fileStreamContent, prop.Name, filePath);
                }
            }
        }

        /*
            send HTTP POST request  
        */
        private bool upload(object query)
        {
            res = client.PostAsync(procURI(query), formData).Result;
            if (streams != null)
            {
                foreach (FileStream stream in streams)
                    stream.Close();
                streams = null;
            }
            formData.Dispose();
            client.Dispose();
            response = res.Content.ReadAsStringAsync().Result;
            return res.IsSuccessStatusCode;
        }

        /*
            sned HTTP Request (JSON format contnet) 
        */
        private bool send(bool saveBinary = false)
        {
            try
            {
                webResponse = req.GetResponse();
                statusCode = (int)((HttpWebResponse)webResponse).StatusCode;
                if (saveBinary)
                    using (var ms = new MemoryStream())
                    {
                        var stream = webResponse.GetResponseStream();

                        byte[] buffer = new byte[binarySize];
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        binaryResponse = ms.ToArray();
                    }
                else
                    using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
            }
            catch (WebException e)
            {
                log.write("Fail WebRequest URL: " + uri);
                log.write("Fail WebRequest Data: " + current_data);
                if (e.Response != null)
                    using (var errorResponse = (HttpWebResponse)e.Response)
                    {
                        statusCode = (int)errorResponse.StatusCode;
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            response = error;
                            log.write(error);
                        }
                    }
                return false;
            }
            return true;
        }

        /*
            mapping response to Object 
        */
        private T getResponse<T>()
        {
            Trace.WriteLine("[Debug] Fetch getResponse<T>: " + response);
            return JsonConvert.DeserializeObject<T>(response);
        }

        /*
            directly return response data
        */
        public string getResponse()
        {
            return response;
        }

        /*
            directly return response data
        */

        public byte[] getBinaryResponse()
        {
            return binaryResponse;
        }

        /*
            directly return status code
        */
        public int getStatusCode()
        {
            return statusCode;
        }
        /*
           directly return ResponseHeader
       */
        public WebHeaderCollection getResponseHeaders()
        {
            return webResponse != null ? webResponse.Headers : null;
        }
        /*
            return ResponseHeader by key
       */
        public string getResponseHeader(string key)
        {
            try
            {
                return webResponse != null ? webResponse.Headers[key] : null;
            }
            catch(Exception e)
            {
                log.write(e.ToString());
                return null;
            }
        }
        /*
            append query string by QueryString object to URL
        */
        private string procURI(object query)
        {
            current_data = JsonConvert.SerializeObject(query);
            string newurl = query == null ? uri : (uri + "?" + QueryString.Parse(query));
            Trace.WriteLine("[Debug] Fetch procURI<T>: " + newurl);
            return newurl;
        }
    }
}
