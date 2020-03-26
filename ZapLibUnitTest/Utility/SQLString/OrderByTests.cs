using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ZapLib.Utility.SQLString.Tests
{
    [TestClass()]
    public class OrderByTests
    {
        [TestMethod()]
        public void WhiteListTest()
        {
            string sort_sql = "Name desc, desc asc, Age,    phone ask";
            string res = OrderBy.WhiteList(new string[] { "name", "age", "phone" })(sort_sql, "name desc");
            Trace.WriteLine(res);
            Assert.AreEqual("name desc,age,phone",res);
            res = OrderBy.WhiteList(null)(null, "name desc");
            Trace.WriteLine(res);
            Assert.AreEqual("name desc", res);
        }
    }
}