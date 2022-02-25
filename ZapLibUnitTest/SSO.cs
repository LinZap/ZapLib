using Interface_SSO;
using Interface_SSO.Model;
using System.Web.Http.Controllers;

namespace ZapLibUnitTest
{
    public class SSO : ExtApiSSO
    {
        public SSO(HttpActionContext api) : base(api) { }
        public override ModelSSOLogin Login() { return new ModelSSOLogin(); }
        public override (bool result, string errmsg) Logout() { return (true, ""); }
    }
}
