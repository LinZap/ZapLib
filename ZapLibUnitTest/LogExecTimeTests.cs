using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZapLib.Tests
{
    [TestClass()]
    public class LogExecTimeTests
    {
        [TestMethod()]
        public void LogTest()
        {
            LogExecTime lxt = new LogExecTime("Unit Test");
            Thread.Sleep(1500);
            lxt.Log();
            Assert.IsNotNull(lxt);
        }
    }
}