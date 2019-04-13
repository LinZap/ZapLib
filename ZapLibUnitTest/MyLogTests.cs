using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ZapLib.Utility;

namespace ZapLib.Tests
{
    [TestClass()]
    public class MyLogTests
    {
        [TestMethod()]
        public void write()
        {
            MyLog log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            log.Path = "D:\\Log";
            log.Write("wewfewfewfewfewf");
            log.Write("safawfqafw");
            Assert.IsNotNull(log);
        }

        [TestMethod()]
        public void write2()
        {
            MyLog log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            log.Path = "D:\\Log";
            log.Name = "MyProcessLogFile";

            log.Write("程式開始...");
            Thread.Sleep(3000);
            log.Write("程式結束...");

            Assert.IsNotNull(log);
        }


        [TestMethod()]
        public void forcewrite()
        {
            FileInfo file = new FileInfo(@"D:\Storage\20190321.txt");
            var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            MyLog log = new MyLog();
            log.Name = "20190321";
            log.Write("程式開始...");
            log.Write("程式結束...");
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
                log.SilentMode = Config.Get("SilentMode");
                log.Write(e.ToString());
            }
        }


    }
}