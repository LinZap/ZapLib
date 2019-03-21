using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ZapLib.Tests
{
    [TestClass()]
    public class MyLogTests
    {
        [TestMethod()]
        public void write()
        {
            MyLog log = new MyLog();
            log.silentMode = Config.get("SilentMode");
            log.path = "D:\\Log";
            log.write("wewfewfewfewfewf");
            log.write("safawfqafw");
            Assert.IsNotNull(log);
        }

        [TestMethod()]
        public void write2()
        {
            MyLog log = new MyLog();
            log.silentMode = Config.get("SilentMode");
            log.path = "D:\\Log";
            log.name = "MyProcessLogFile";

            log.write("程式開始...");
            Thread.Sleep(3000);
            log.write("程式結束...");

            Assert.IsNotNull(log);
        }


        [TestMethod()]
        public void forcewrite()
        {
            FileInfo file = new FileInfo(@"D:\Storage\20190321.txt");
            var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            MyLog log = new MyLog();
            log.name = "20190321";
            log.write("程式開始...");
            log.write("程式結束...");
            if (stream != null) stream.Close();
            Assert.IsNotNull(log);
        }

        [TestMethod()]
        public void write1()
        {
            try
            {
                int x = 0;
                int i = 100 / x;
                Console.WriteLine(i);
            }
            catch(Exception e)
            {
                MyLog log = new MyLog();
                log.silentMode = Config.get("SilentMode");
                log.write(e.ToString());
            }
        }


    }
}