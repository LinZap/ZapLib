using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ZapLib.Tests
{
    [TestClass()]
    public class FetchTests
    {
        [TestMethod()]
        public void post()
        {

            Dictionary<string, string> file = new Dictionary<string, string>();
            Dictionary<string, string> data = new Dictionary<string, string>();

            file.Add("file", @"C:\Users\zaplin\Downloads\caret-down.png");
            data.Add("test", "123");

            Fetch f = new Fetch("https://httpbin.org/post");
            //f.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            var res = f.Post(data, new { test = "123" }, file);


            Trace.WriteLine(res);
        }


        [TestMethod()]
        public void post2()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("test", "123");
            data.Add("GG", "wefewfhnwoefoew");
            Fetch f = new Fetch("https://httpbin.org/get");
            //f.contentType = "application/x-www-form-urlencoded";
            var res = f.Post(data, new { test = "123" });
            Trace.WriteLine(res);
        }

        [TestMethod()]
        public void Send()
        {
            Fetch f = new Fetch("http://192.168.1.102/api/core/me");
            string res = f.Get();
            Trace.WriteLine(f.GetResponse());
            Assert.IsNull(res);
        }

        [TestMethod()]
        public void Fetch()
        {
            Fetch f = new Fetch("http://m10.music.126.net/20190603190710/cc734d7794b20778f8a83284cc346ce7/ymusic/c230/0fd0/65f4/576d2b12e08c6d5b2164a8f02a6604de.mp3");
            f.Proxy = "http://127.0.0.1:11080";
            string res = f.Get();
            Trace.WriteLine(res);
            Trace.WriteLine("---------------------------------");
            Trace.WriteLine(f.GetResponse());
            Assert.IsNull(res);
        }

        [TestMethod()]
        public void Get()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Get1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBinary()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Post()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Post1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Delete()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Delete1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Put()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Put1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetResponse()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBinaryResponse()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetResponseHeaders()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetResponseHeader()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BuildMultipartFormDataContent()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BuildFormUrlEncodedContent()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BuildStringContent()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Send1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Dispose()
        {
            Assert.Fail();
        }
    }
}