using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ZapLib.Tests
{
    [TestClass()]
    public class FetchTests
    {
        [TestMethod()]
        public void post()
        {
            string passportcode = "ukQvwRHcfHLxmIHbvBJxpO5jcJOaatlK6hpTJhfT5yd5Zfd5Tdr62vm69azrl65nh2Dnn42nr6Xyy49ytF9lj82zayf84djQDdw78vuU1wbUXuuX9flX7zgmtA7kxA7hwXOuwAJhfEOblWPuwRWeonwIRqrHRnpRBrmRMrnJJswKVvqKLaavhV0nhVKtjTLlm3MxsYNvm0YcdY9igxb6QhsM2yh6SspZ8rmNQcb46lq4XztcNA";
            Fetch f = new Fetch("https://csweb.iqs-t.com/api/core/me");
            f.Cookie = new { passportcode };
            string response = f.Get();

           
            var cs = f.ClientHandler.CookieContainer.GetCookies(f.Client.BaseAddress).Cast<Cookie>();
            foreach (var c in cs)
                Trace.WriteLine(c.Name + ":" + c.Value);
        }


        [TestMethod()]
        public void post2()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("test", "123");
            data.Add("GG", "wefewfhnwoefoew");
            Fetch f = new Fetch("htts://httpbin.org/get");
            //f.contentType = "application/x-www-form-urlencoded";
            var res = f.Post(data, new { test = "123" });
            Trace.WriteLine(res);
        }

        [TestMethod()]
        public void post3()
        {
            Fetch f = new Fetch("http://10.9.173.102:8888/api/upload");
            f.ValidPlatform = true;
            object res = f.Post<object>(new
            {
                name = "123.txt",
                file = File.ReadAllBytes("D:\\123.txt")
            });

            Trace.WriteLine(JsonConvert.SerializeObject(res));
            
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
            Fetch f = new Fetch("http://192.168.1.102:8888/api/search?q=%E9%9F%93");
            string res = f.Get();
            Trace.WriteLine(f.Url);
            Trace.WriteLine(f.StatusCode);
            Trace.WriteLine(res);
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
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            Fetch f = new Fetch("https://miro.medium.com/max/676/1*XEgA1TTwXa5AvAdw40GFow.png");
            var res = f.GetBinary();
            Assert.IsNotNull(res);
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

    public class ModelUpload
    {
        public string name { get; set; }
        public byte[] file { get; set; }
    }
}