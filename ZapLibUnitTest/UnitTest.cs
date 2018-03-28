using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZapLibUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string mime = MimeMapping.GetMimeMapping("efjewfiewfjew.xls");
            Console.WriteLine("efjewfiewfjew.xls" + " " + mime);

            mime = MimeMapping.GetMimeMapping("we.png");
            Console.WriteLine("we.png" + " " + mime);
            Assert.IsNotNull(mime);
        }
    }
}
