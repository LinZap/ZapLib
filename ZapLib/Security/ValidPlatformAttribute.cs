using System;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net;
using Newtonsoft.Json;

namespace ZapLib.Security
{
    /// <summary>
    /// 平台驗證標籤，加上該標籤的 Action 都會進行平台驗證檢查，檢查失敗將回傳 401 Unauthorized，且不會進入 Action
    /// </summary>
    public class ValidPlatformAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Web API 進入 Action 會執行這個方法驗證
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var header = actionContext.Request.Headers;
            string Signature = getHeader(header, "Channel-Signature");
            string IV = getHeader(header, "Channel-Iv");
            string Authorization = getHeader(header, "Channel-Authorization");

            if (!string.IsNullOrWhiteSpace(Authorization))
            {
                if (Authorization == Const.GodKey) return;
                Crypto crypto = new Crypto();
                if (crypto.DESEncryption(Signature, IV) == Authorization) return;
            }

            HttpRequestMessage Request = actionContext.Request;
            HttpStatusCode httpcode = HttpStatusCode.Unauthorized;
            HttpResponseMessage Response = Request.CreateResponse(httpcode);
            Response.Content = new StringContent(
                JsonConvert.SerializeObject(new { error = "Request is not valid, please check [ValidPlatform]" }), Encoding.UTF8, "application/json");
            actionContext.Response = Response;

        }
        private string getHeader(HttpRequestHeaders header, string key)
        {
            try
            {
                return header.GetValues(key).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
