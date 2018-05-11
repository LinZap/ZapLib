using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using ZapLib.Model;

namespace ZapLib
{
    public class ExtApiHelper
    {
        private ApiController api;
        private HttpRequestMessage request;
        private HttpResponseMessage resp;
        private List<CookieHeaderValue> cookies;
        private Dictionary<string, string> queries;

        public ExtApiHelper(ApiController api)
        {
            this.api = api;
            request = api.Request;
            cookies = new List<CookieHeaderValue>();
            resp = request.CreateResponse();
        }

        /*
            get header from Http Request
        */
        public string getHeader(string key)
        {
            try
            {
                return request.Headers.GetValues(key).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*
            set header 
        */
        public void setHeader(string key, string value)
        {
            resp.Headers.Add(key, value);
        }


        /*
            get ip from Http Request 
        */
        public string getIP()
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }


        /*
            get user agent from HTTP Request 
        */
        public string getUserAgent()
        {
            try
            {
                return request.Headers.UserAgent.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /*
            get urlprifix  scheme://hostname:port
        */
        public string getMyHost()
        {
            return string.Format("{0}://{1}:{2}", request.RequestUri.Scheme, request.RequestUri.Host, request.RequestUri.Port);
        }


        /*
            create cookie object
        */
        public CookieHeaderValue addCookie(string key, string value, DateTime expired)
        {
            var cookie = new CookieHeaderValue(key, value);
            DateTimeOffset expiredOffset = DateTime.SpecifyKind(expired, DateTimeKind.Utc);
            cookie.Expires = expiredOffset;
            cookie.Domain = request.RequestUri.Host == "localhost" ? null : request.RequestUri.Host;
            cookie.Path = "/";
            cookie.HttpOnly = true;
            cookies.Add(cookie);
            return cookie;
        }

        /*
            get cookie
            reurn value or null
        */
        public string getCookie(string key)
        {
            CookieHeaderValue c = api.Request.Headers.GetCookies(key).FirstOrDefault();
            return c == null ? null : c[key].Value;
        }

        /*
            response custom response   
        */
        public HttpResponseMessage getResponse(object content = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            resp.Headers.AddCookies(cookies.ToArray());
            if (content != null)
                resp.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return resp;
        }

        /*
            response custom response   
        */
        public HttpResponseMessage getTextResponse(string content = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            resp.Headers.AddCookies(cookies.ToArray());
            if (content != null)
                resp.Content = new StringContent(content, Encoding.UTF8, "text/html");
            return resp;
        }

        /*
           set redirect URL 
       */
        public HttpResponseMessage getRedirectResponse(string url, int second = 1, string wording = "跳轉中，請稍候...")
        {
            resp.Headers.AddCookies(cookies.ToArray());
            resp.Headers.Location = new Uri(url);
            string html = "<meta http-equiv=\"refresh\" content=\"{1}; URL = '{0}'\" /><body>{2}</body>";
            resp.Content = new StringContent(string.Format(html, url, second, wording), Encoding.UTF8, "text/html");
            return resp;
        }

        /*
            response text using attachment (download)
        */
        public HttpResponseMessage getAttachmentResponse(string content, string filename = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            string fn = (filename ?? Guid.NewGuid().ToString());
            if (content != null)
            {
                string mimeType = MimeMapping.GetMimeMapping(fn) ?? "application/octet-stream";
                resp.Content = new StringContent(content, Encoding.UTF8, mimeType);
                resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fn
                };
            }
            return resp;
        }
        /*
           get query value by key from request query string 
           if can not get value, return null
        */
        public string getQuery(string key)
        {
            if (queries == null)
            {
                queries = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kv in api.Request.GetQueryNameValuePairs())
                {
                    queries[kv.Key] = kv.Value;
                }
            }
            string val = null;
            return queries.TryGetValue(key, out val) ? val : null;
        }

        /*
            add paging sql
        */
        public void addPaging(ref string sql, string orderby = "asc")
        {
            int sysLimit = int.TryParse(Config.get("APIDataLimit"), out sysLimit) ? sysLimit : 50;
            int ilimit = int.TryParse(getQuery("limit"), out ilimit) ? ilimit : 50;

            ilimit = Math.Min(sysLimit, ilimit);

            int ipage = int.TryParse(getQuery("page"), out ipage) ? ipage : 1;
            ipage = Math.Max(1, ipage);

            /*
             *  [this function is running on sql server 2012 (or upper)]               
                int offset = (ipage - 1) * ilimit;
                sql += string.Format(" order by {0} offset {1} rows fetch next {2} rows only", orderby, offset, ilimit);
            */

            int start = (ipage - 1) * ilimit + 1,
                end = start + ilimit - 1;

            Regex reg = new Regex("^select ");
            string replacement = string.Format("select ROW_NUMBER() over({0}) as rownumber,", "order by " + orderby),
                   new_sql = reg.Replace(sql, replacement);
            sql = string.Format("with tb as({0})select * from tb where rownumber between {1} and {2}", new_sql, start, end);
        }


        /*
            upload file from multi-part form
            if can not get any file or all of files are too large , return null
            otherwise return List<ModelFile>
                @name: new file name (rename)
                @des: old file name
                @size: file size (byte)
                @type: file's mime type
        */
        public List<ModelFile> uploadFile()
        {
            string dest = Config.get("Storage");
            long maxSize = long.TryParse(Config.get("MaxUploadFileSize"), out maxSize) ? maxSize : 5242880;
            MyLog log = new MyLog();
            HttpFileCollection files = HttpContext.Current.Request.Files;

            if (files.Count < 1) return null;

            var filelist = new List<ModelFile>();

            foreach (string key in files)
            {
                HttpPostedFile file = files[key];
                string fileName = Path.GetFileName(file.FileName);
                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
                string newName = string.Format("{0}_{1}", Guid.NewGuid().ToString(), fileName);
                string destPath = string.Format("{0}/{1}", dest, newName);
                file.SaveAs(path);
                FileInfo info = new FileInfo(path);
                // size filter
                if (info.Length > maxSize)
                {
                    log.write("file " + fileName + " is too large: " + info.Length + ". be deleted!");
                    File.Delete(path);
                    continue;
                }
                File.Move(path, destPath);
                filelist.Add(new ModelFile()
                {
                    name = newName,
                    oid = 0,
                    des = fileName,
                    size = info.Length,
                    type = MimeMapping.GetMimeMapping(fileName)
                });
            }
            return filelist.Count < 1 ? null : filelist;
        }

        /*
            get body from form and map it to Object params 
        */
        public T getFormBody<T>()
        {
            NameValueCollection form = HttpContext.Current.Request.Form;
            T obj = (T)Activator.CreateInstance(typeof(T));
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        prop.SetValue(obj, form.Get(prop.Name), null);
                    }
                    catch (Exception) { }
                }
            }
            return obj;
        }

        /*
            get body from raw json and map it to Object params 
        */
        public T getJsonBody<T>()
        {
            string s = null;
            HttpRequest Request = HttpContext.Current.Request;
            if (Request.TotalBytes > 0)
                s = Encoding.GetEncoding("utf-8").GetString(Request.BinaryRead(Request.TotalBytes));
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
