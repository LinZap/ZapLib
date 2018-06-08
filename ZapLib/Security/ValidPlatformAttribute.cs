using System;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ZapLib.Security
{
    public class ValidPlatformAttribute : AuthorizeAttribute
    {
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
