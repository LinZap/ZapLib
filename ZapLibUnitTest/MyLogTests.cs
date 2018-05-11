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
        public void write()
        {
            MyLog log = new MyLog();
            log.path = "D:\\Log";
            log.write("wewfewfewfewfewf");
            log.write("safawfqafw");
            Assert.IsNotNull(log);
        }

        [TestMethod()]
        public void write2()
        {
            MyLog log = new MyLog();
            log.path = "D:\\Log";
            log.name = "mylog";
            log.write("wewfewfewfewfewf");
            log.write("safawfqafw");
            Assert.IsNotNull(log);
        }
    }
}