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
        public void addPagingTest()
        {
            ExtApiHelper api = new ExtApiHelper(new TestController());
            string sql = "select * from entity where eid>10";
            api.addPaging(ref sql, "eid desc");
            Trace.WriteLine(sql);
            StringAssert.Contains(sql, "select * from tb where rownumber between");
        }

        [TestMethod()]
        public void addIdentityPagingTest()
        {
            ExtApiHelper api = new ExtApiHelper(new TestController());
            string sql = "select * from entity where eid>10";
            api.addIdentityPaging(ref sql, "eid desc", "eid", null);

            string sql2 = "select * from entity where eid>10";
            api.addIdentityPaging(ref sql2, "eid desc", "eid", "2");

            Trace.WriteLine(sql);
            Trace.WriteLine(sql2);

            Assert.IsFalse(sql.Contains("ROW_NUMBER"));
            StringAssert.Contains(sql2, "ROW_NUMBER()");
        }

    }
    public class TestController : ApiController { }
}