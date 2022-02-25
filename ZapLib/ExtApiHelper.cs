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
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// Web API 控制存取輔助工具
    /// </summary>
    public class ExtApiHelper
    {
        /// <summary>
        /// 初始請求物件
        /// </summary>
        public HttpRequestMessage request { get; private set; }
        /// <summary>
        /// 最終回應物件
        /// </summary>
        public HttpResponseMessage resp { get; private set; }

        /// <summary>
        /// 設定在 response 的 cookie, 只允許在內部 AddCookie 進行設定
        /// </summary>
        public List<CookieHeaderValue> cookies { private set; get; }

        /// <summary>
        /// Response 的 Encoding，預設  Encoding.UTF8
        /// </summary>
        public Encoding encoding { get; set; } = Encoding.UTF8;

        private Dictionary<string, string> queries;

        /// <summary>
        /// 建構子，請傳入目前的 ApiController (this) 實體
        /// </summary>
        /// <param name="api">目前這個 Controller 物件</param>
        public ExtApiHelper(ApiController api = null)
        {
            if (api == null) return;
            request = api?.Request ?? new HttpRequestMessage();
            cookies = new List<CookieHeaderValue>();
            resp = request == null ? new HttpResponseMessage() : request.CreateResponse();
        }

        /// <summary>
        /// 從客戶的請求中取出所有 Headers
        /// </summary>
        /// <returns>所有 Header 的字典集合</returns>
        public Dictionary<string, IEnumerable<string>> GetHeaders()
        {
            Dictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
            try
            {
                foreach (var header in request.Headers)
                {
                    headers.Add(header.Key, header.Value);
                }
            }
            catch { }
            return headers;
        }

        /// <summary>
        /// 從客戶的請求中取出指定名稱的 Header 數值，如果取不到則回傳預設值
        /// </summary>
        /// <param name="key">指定名稱</param>
        /// <param name="def_val">預設數值</param>
        /// <returns>Header 數值，取不到時回傳 NULL</returns>
        public string GetHeader(string key, string def_val = null)
        {
            try
            {
                return request.Headers.GetValues(key).FirstOrDefault();
            }
            catch (Exception)
            {
                return def_val;
            }
        }


        /// <summary>
        /// 從客戶的請求中取出指定名稱的 Header 數值，並嘗試轉換成指定型態 T，如果取不到或轉換不過則回傳預設值
        /// </summary>
        /// <typeparam name="T">指定轉換的型態</typeparam>
        /// <param name="key">指定名稱</param>
        /// <param name="def_val">預設數值</param>
        /// <returns></returns>
        public T GetHeader<T>(string key, T def_val = default)
        {
            try
            {
                return Cast.To(request.Headers.GetValues(key).FirstOrDefault(), def_val);
            }
            catch (Exception)
            {
                return def_val;
            }
        }

        /// <summary>
        /// 設定回應內容的 Header 數值
        /// </summary>
        /// <param name="key">名稱</param>
        /// <param name="value">數值</param>
        public void SetHeader(string key, string value)
        {
            resp.Headers.Add(key, value);
        }


        /// <summary>
        /// 取得客戶請求的來源 IP
        /// </summary>
        /// <returns>IP 字串</returns>
        public string GetIP()
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


        /// <summary>
        /// 取得客戶請求的瀏覽器版本
        /// </summary>
        /// <returns>瀏覽器版本字串</returns>
        public string GetUserAgent()
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

        /// <summary>
        /// 取得客戶請求的 URL 字串
        /// </summary>
        /// <returns>URL 字串</returns>
        public string GetMyHost()
        {
            return string.Format("{0}://{1}:{2}", request.RequestUri.Scheme, request.RequestUri.Host, request.RequestUri.Port);
        }


        /// <summary>
        /// 設定回應內容的 Cookie 數值
        /// </summary>
        /// <param name="key">名稱</param>
        /// <param name="value">數值</param>
        /// <param name="expired">過期時間</param>
        /// <returns>設定好的 Cookie 物件</returns>
        public CookieHeaderValue AddCookie(string key, string value, DateTime expired)
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

        /// <summary>
        /// 取得客戶請求中特定名稱的 Cookie 數值
        /// </summary>
        /// <param name="key">特定名稱</param>
        /// <returns>Cookie 數值，取不到時為 NULL</returns>
        public string GetCookie(string key)
        {
            CookieHeaderValue c = request.Headers.GetCookies(key).FirstOrDefault();
            return c == null ? null : c[key].Value;
        }

        /// <summary>
        /// 取得 API 回應物件，並將內容物件邊碼成 JSON 格式
        /// </summary>
        /// <param name="content">內容物件</param>
        /// <param name="code">回應的 HTTP 狀態碼，預設 200</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetResponse(object content = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            resp.Headers.AddCookies(cookies.ToArray());
            if (content != null)
                resp.Content = new StringContent(JsonConvert.SerializeObject(content), encoding, "application/json");
            return resp;
        }

        /// <summary>
        /// 取得 API 回應物件，並將內容保持純文字格式
        /// </summary>
        /// <param name="content">內容物件</param>
        /// <param name="code">回應的 HTTP 狀態碼，預設 200</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetTextResponse(string content = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            resp.Headers.AddCookies(cookies.ToArray());
            if (content != null)
                resp.Content = new StringContent(content, encoding, "text/html");
            return resp;
        }

        /// <summary>
        /// 取得 API 回應物件，並將使用者導向到新的 URL 位置
        /// </summary>
        /// <param name="url">導向到新的 URL</param>
        /// <param name="second">延遲秒數，預設 1 秒</param>
        /// <param name="wording">顯示文字，預設為 "跳轉中，請稍候..."</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetRedirectResponse(string url, int second = 1, string wording = "跳轉中，請稍候...")
        {
            resp.Headers.AddCookies(cookies.ToArray());
            resp.Headers.Location = new Uri(url);
            string html = "<meta http-equiv=\"refresh\" content=\"{1}; URL = '{0}'\" /><body>{2}</body>";
            resp.Content = new StringContent(string.Format(html, url, second, wording), encoding, "text/html");
            return resp;
        }

        /// <summary>
        /// 取得 API 回應物件，並將內容字串以串流方式回應 (客戶將以下載檔案方式取得文字)
        /// </summary>
        /// <param name="content">內容字串</param>
        /// <param name="filename">檔案名稱</param>
        /// <param name="code">回應的 HTTP 狀態碼，預設 200</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetAttachmentResponse(string content, string filename = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            resp.StatusCode = code;
            string fn = (filename ?? Guid.NewGuid().ToString());
            if (content != null)
            {
                string mimeType = MimeMapping.GetMimeMapping(fn) ?? "application/octet-stream";
                resp.Content = new StringContent(content, encoding, mimeType);
                resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fn
                };
            }
            return resp;
        }


        /// <summary>
        /// 取得 API 回應物件，並將指定路徑的檔案以串流方式回應 (客戶將以下載檔案方式取得檔案)
        /// </summary>
        /// <param name="file">檔案路徑，當檔案不存在時將回傳 404 Not Found 的 Status Code</param>
        /// <param name="name">檔案名稱，預設為隨機產生亂碼</param>
        /// <param name="type">回應的內容格式，預設為 application/octet-stream</param>
        /// <param name="disposition">下載模式，預設為 attachment ，強迫以下載方式取得檔案</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetStreamResponse(string file, string name = null, string type = "application/octet-stream", string disposition = "attachment")
        {
            string fn = (name ?? Guid.NewGuid().ToString());

            if (File.Exists(file))
            {
                using (var fs = File.OpenRead(file))
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = new StreamContent(fs);
                    resp.Content.Headers.ContentType = new MediaTypeHeaderValue(type);
                    resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
                    {
                        FileName = fn
                    };
                    return resp;
                }
            }
            else
            {
                resp.StatusCode = HttpStatusCode.NotFound;
                resp.Content = new StringContent("NotFound");
                return resp;
            }
        }

        /// <summary>
        /// 取得 API 回應物件，並將指定路徑的檔案以串流方式回應 (客戶將以下載檔案方式取得檔案)
        /// </summary>
        /// <param name="stream">檔案串流，當串流無法讀取時 404 Not Found 的 Status Code</param>
        /// <param name="name">檔案名稱，預設為隨機產生亂碼</param>
        /// <param name="type">回應的內容格式，預設為 application/octet-stream</param>
        /// <param name="disposition">下載模式，預設為 attachment ，強迫以下載方式取得檔案</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetStreamResponse(Stream stream, string name = null, string type = "application/octet-stream", string disposition = "attachment")
        {
            string fn = (name ?? Guid.NewGuid().ToString());
            if (stream.CanRead)
            {
                using (var sc = new StreamContent(stream))
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content = sc;
                    resp.Content.Headers.ContentType = new MediaTypeHeaderValue(type);
                    resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
                    {
                        FileName = fn
                    };
                    return resp;
                }
            }
            else
            {
                resp.StatusCode = HttpStatusCode.NotFound;
                resp.Content = new StringContent("Stream can not read!");
                return resp;
            }

        }

        /// <summary>
        /// 取得 API 回應物件，並將指定byte[] 二進位資料以串流方式回應 (客戶將以下載檔案方式取得資料)
        /// </summary>
        /// <param name="file">二進位資料</param>
        /// <param name="name">檔案名稱，預設為隨機產生亂碼</param>
        /// <param name="type">回應的內容格式，預設為 application/octet-stream</param>
        /// <param name="disposition">下載模式，預設為 attachment ，強迫以下載方式取得檔案</param>
        /// <returns>Web API 回應物件</returns>
        public HttpResponseMessage GetStreamResponse(byte[] file, string name = null, string type = "application/octet-stream", string disposition = "attachment")
        {
            string fn = (name ?? Guid.NewGuid().ToString());
            resp.StatusCode = HttpStatusCode.OK;
            resp.Content = new ByteArrayContent(file);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(type);
            resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
            {
                FileName = fn
            };
            return resp;
        }

        /// <summary>
        /// 取得客戶請求查詢字串中指定的名稱數值
        /// </summary>
        /// <param name="key">指定名稱</param>
        /// <returns>指定名稱的數值</returns>
        public string GetQuery(string key)
        {
            if (queries == null)
            {
                queries = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kv in request.GetQueryNameValuePairs())
                {
                    queries[kv.Key] = kv.Value;
                }
            }
            string val = null;
            return queries.TryGetValue(key, out val) ? val : null;
        }

        /// <summary>
        /// 增加 SQL 翻頁語法
        /// </summary>
        /// <param name="sql">SQL 語法</param>
        /// <param name="orderby">欄位名稱</param>
        [Obsolete("這個方法可能在下個版本中棄用")]
        public void AddPaging(ref string sql, string orderby = "asc")
        {
            int sysLimit = int.TryParse(Config.Get("APIDataLimit"), out sysLimit) ? sysLimit : 50;
            int ilimit = int.TryParse(GetQuery("limit"), out ilimit) ? ilimit : 50;

            ilimit = Math.Min(sysLimit, ilimit);

            int ipage = int.TryParse(GetQuery("page"), out ipage) ? ipage : 1;
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

        /// <summary>
        /// 增加 SQL 以某個具備識別的欄位數值為基準進行翻頁的語法
        /// </summary>
        /// <param name="sql">SQL 語法</param>
        /// <param name="orderby">欄位名稱</param>
        /// <param name="idcolumn">具備識別的欄位名稱</param>
        /// <param name="nextId">翻頁 ID</param>
        public void AddIdentityPaging(ref string sql, string orderby = "since desc", string idcolumn = null, string nextId = null)
        {
            bool isFirstPage = (string.IsNullOrEmpty(idcolumn) || string.IsNullOrEmpty(nextId));
            int sysLimit = int.TryParse(Config.Get("APIDataLimit"), out sysLimit) ? sysLimit : 50;
            int ilimit = int.TryParse(GetQuery("limit"), out ilimit) ? ilimit : 50;
            ilimit = Math.Min(sysLimit, ilimit) + 1;
            Regex reg = new Regex("^select ");
            string replacement = isFirstPage ? string.Format("select top({0}) ", ilimit) :
                                               string.Format("select ROW_NUMBER() over(order by {0}) as _seq,", orderby),
                  new_sql = reg.Replace(sql, replacement);
            sql = isFirstPage ? string.Format("{0} order by {1}", new_sql, orderby) :
                                string.Format("with tb as({0}) select top({1}) * from tb where _seq>=(select _seq from tb where {2}='{3}') order by {4}", new_sql, ilimit, idcolumn, nextId, orderby);
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
        /// <summary>
        /// 將客戶請求中的 multi-part form 檔案部分儲存到 .config 中設定的 Storage 路徑下，並回傳該檔案的基本資訊模型物件
        /// </summary>
        /// <returns>檔案基本資訊資料模型</returns>
        public List<ModelFile> UploadFile()
        {
            string dest = Config.Get("Storage");
            long maxSize = long.TryParse(Config.Get("MaxUploadFileSize"), out maxSize) ? maxSize : 5242880;
            MyLog log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
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
                    log.Write("file " + fileName + " is too large: " + info.Length + ". be deleted!");
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


        /// <summary>
        /// 取得客戶請求中的 multi-part form 資料部分，並反序列化綁定到指定的資料模型中
        /// </summary>
        /// <typeparam name="T">指定的資料模型型態 T</typeparam>
        /// <returns>綁定好資料的資料模型</returns>
        public T GetFormBody<T>()
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

        /// <summary>
        /// 取得客戶請求中的 JSON 資料部分，並反序列化綁定到指定的資料模型中
        /// </summary>
        /// <typeparam name="T">指定的資料模型型態</typeparam>
        /// <returns>綁定好資料的資料模型</returns>
        public T GetJsonBody<T>()
        {
            string s = null;
            HttpRequest Request = HttpContext.Current.Request;

            if (Request.TotalBytes > 0)
            {
                //s = Encoding.GetEncoding("utf-8").GetString(Request.BinaryRead(Request.TotalBytes));
                s = request.Content.ReadAsStringAsync().Result;
            }
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
