using Interface_SSO;
using Interface_SSO.Model;
using System.Web.Http;

namespace ZapLibUnitTest
{
    public class SSO : ExtApiSSO
    {
        public SSO(ApiController api) : base(api) { }
        public override ModelSSOLogin Login() { return new ModelSSOLogin(); }
        public override (bool result, string errmsg) Logout() { return (true, ""); }
    }
}
