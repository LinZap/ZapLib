using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Tests
{
    [TestClass()]
    public class MyLogTests
    {
        [TestMethod()]
        public void writeTest()
        {
            var mylog = new MyLog();
            mylog.name = "測試";
            mylog.path = @"D:\";
            var actual = mylog.Logres();
            Assert.AreEqual("ok",actual);
        }
    }
}