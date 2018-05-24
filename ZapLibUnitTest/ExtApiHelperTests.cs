using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Http;

namespace ZapLib.Tests
{
    [TestClass()]
    public class ExtApiHelperTests
    {
        [TestMethod()]
        public void addIdentityPaging()
        {
            TestController test_controller = new TestController();
            ExtApiHelper api = new ExtApiHelper(test_controller);
            string sql = "select * from entity where eid>10";
            api.addIdentityPaging(ref sql,"eid desc","eid",null);

            string sql2 = "select * from entity where eid>10";
            api.addIdentityPaging(ref sql2, "eid desc", "eid", "2");

            Trace.WriteLine(sql);
            Trace.WriteLine(sql2);
            
        }
    }

    public class TestController : ApiController { }
}