using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using ZapLib.Security;

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
        public bool validPlatform { get; set; } = false;

        public Fetch()
        {
            log = new MyLog();
            log.silentMode = Config.get("SilentMode");
            proxy = Config.get("Proxy");
        }

        public Fetch(string url)
        {
            uri = url;
            log = new MyLog();
            log.silentMode = Config.get("SilentMode");
            proxy = Config.get("Proxy");
        }

        private void appendHeaders()
        {
            if (header == null) return;
            if (header.GetType() == typeof(Dictionary<string, string>))
            {
                foreach (KeyValuePair<string, string> prop in (Dictionary<string, string>)header)
                    appendHeader(prop.Key, prop.Value);
            }
            else
            {
                try
                {
                    foreach (var prop in header.GetType().GetProperties())
                    {
                        string name = prop.Name;
                        object value = prop.GetValue(header, null);
                        if (value != null) appendHeader(name, value.ToString());
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("can not set header:" + JsonConvert.SerializeObject(header));
                    Trace.WriteLine("can not set header:" + e.ToString());
                }
            }

            if (req != null && userAgent != null) ((HttpWebRequest)req).UserAgent = userAgent;
            if (client != null && userAgent != null) client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }


        private void appendHeader(string key, string val)
        {
            if (req != null) req.Headers.Add(key, val);
            if (client != null) client.DefaultRequestHeaders.Add(key, val);
        }

        /*
            call HTTP POST and return response
            (string object) 
        */
        public T post<T>(object data, object query = null, object files = null)
        {
            return _post(data, query, files) ? getResponse<T>() : default(T);
        }

        /*
            call HTTP POST and return response
            (string present) 
        */
        public string post(object data, object query = null, object files = null)
        {
            return _post(data, query, files) ? getResponse() : null;
        }


        private bool _post(object data, object query = null, object files = null)
        {
            bool statusCode;

            if ((files != null) || contentType.Contains("multipart/form-data"))
            {
                setUpload(data, files);
                statusCode = upload(query);
            }
            else
            {
                setPost(data, query);
                statusCode = send();
            }
            return statusCode;
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
                    current_data = isRaw ? data.ToString() :
                        contentType.Contains("x-www-form-urlencoded") ? QueryString.Parse(data) : JsonConvert.SerializeObject(data);
                    streamWriter.Write(current_data);
                    streamWriter.Flush();
                }
            if (validPlatform)
                procValidPlatform(current_data);
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
            if (validPlatform)
                procValidPlatform(current_data);
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
            if (validPlatform)
                procValidPlatform(JsonConvert.SerializeObject(data));
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
                if (data.GetType() == typeof(Dictionary<string, string>))
                {
                    foreach (KeyValuePair<string, string> prop in (Dictionary<string, string>)data)
                    {
                        if (string.IsNullOrWhiteSpace(prop.Value)) continue;
                        StringContent content = new StringContent(prop.Value);
                        formData.Add(content, prop.Key);
                    }
                }
                else
                {
                    foreach (var prop in data.GetType().GetProperties())
                    {
                        if (prop.GetValue(data, null) == null) continue;
                        StringContent content = new StringContent(prop.GetValue(data, null).ToString());
                        formData.Add(content, prop.Name);
                    }
                }

            }
            // add file data to form      
            if (files != null)
            {
                streams = new List<FileStream>();
                if (files.GetType() == typeof(Dictionary<string, string>))
                {
                    foreach (KeyValuePair<string, string> prop in (Dictionary<string, string>)files)
                    {
                        string filePath = prop.Value;
                        if(filePath==null) continue;
                        filePath = filePath.Trim();
                        if (!File.Exists(filePath)) continue;
                        FileStream fileStream = File.OpenRead(filePath);
                        streams.Add(fileStream);
                        HttpContent fileStreamContent = new StreamContent(fileStream);
                        formData.Add(fileStreamContent, prop.Key, filePath);
                    }
                }
                else
                {
                    foreach (var prop in files.GetType().GetProperties())
                    {
                        string filePath = prop.GetValue(files, null).ToString();
                        if (filePath == null) continue;
                        filePath = filePath.Trim();
                        if (!File.Exists(filePath)) continue;
                        FileStream fileStream = File.OpenRead(filePath);
                        streams.Add(fileStream);
                        HttpContent fileStreamContent = new StreamContent(fileStream);
                        formData.Add(fileStreamContent, prop.Name, filePath);
                    }
                }
            }
            if (validPlatform) procValidPlatform(formData.ReadAsStringAsync().Result);
        }


        public void procValidPlatform(string content)
        {
            Crypto crypto = new Crypto();
            string Signature = crypto.Md5(content);
            string Authorization = crypto.DESEncryption(Signature);
            string IV = crypto.iv;
            appendHeader("Channel-Signature", Signature);
            appendHeader("Channel-Authorization", Authorization);
            appendHeader("Channel-Iv", IV);
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
            catch (Exception e)
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
