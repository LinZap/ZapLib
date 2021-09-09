using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using ZapLib.Utility;
using System.Reflection;
using ZapLib.Utility.Tests;

namespace ZapLib.Tests
{


    [TestClass()]
    public class ClassMirrorTests
    {
        [TestMethod()]
        public void ClassMirrorTest()
        {

            foreach (Type t in Mirror.GetClasses<Interface_SSO.ExtApiSSO>(false))
            {
                Trace.WriteLine("Type: " + t.Name);
                Assert.IsNotNull(t);
                ApiController api = null;
                ClassMirror c = new ClassMirror(t, api);
                Interface_SSO.ExtApiSSO sso = (Interface_SSO.ExtApiSSO)c.Instance;
                if (sso == null)
                {
                    Trace.WriteLine(c.ErrMsg);
                    continue;
                }
                Interface_SSO.Model.ModelSSOLogin data = sso.Login();
                Trace.WriteLine(JsonConvert.SerializeObject(data));
                (bool result, string errmsg) = sso.Logout();
                Trace.WriteLine(new
                {
                    result,
                    errmsg
                });

            }
        }

        [TestMethod()]
        public void ClassMirrorTest1()
        {
            Type sso_class = Mirror.GetClasses<Interface_SSO.ExtApiSSO>().FirstOrDefault();
            if (sso_class != null)
            {
                ApiController api = null;
                ClassMirror cm = new ClassMirror(sso_class, api);

                Interface_SSO.ExtApiSSO sso = (Interface_SSO.ExtApiSSO)cm.Instance;
                if (sso == null)
                {
                    Trace.WriteLine(cm.ErrMsg);
                }
                else
                {
                    Interface_SSO.Model.ModelSSOLogin data = sso.Login();
                    (bool result, string errmsg) = sso.Logout();

                    Trace.WriteLine(JsonConvert.SerializeObject(data));
                    Trace.WriteLine(new { result, errmsg });
                }
            }
        }
    }


}