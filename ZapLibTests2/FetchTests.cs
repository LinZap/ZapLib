using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ZapLib.Tests
{
    [TestClass()]
    public class FetchTests
    {
        [TestMethod()]
        public void postReturnStringTest()
        {
            Fetch f = new Fetch("http://httpbin.org/post");
            f.header = new { cookie = "test=cookieval", test = "headerval" };
            var data = new { test = "json data", version = 1, now = DateTime.Now, istest = true };
            var query = new { token = Guid.NewGuid().ToString(), test = "querystring" };

            string response = f.post(data, query);
            Trace.WriteLine(response);
            Assert.IsNotNull(response);
            StringAssert.Contains(response, "headerval");
            StringAssert.Contains(response, "querystring");
            StringAssert.Contains(response, "json data");
        }

        [TestMethod()]
        public void postReturnModelTest()
        {
            Fetch f = new Fetch("http://httpbin.org/post");
            f.header = new { cookie = "test=cookieval", test = "headerval" };
            var data = new { test = "json data" };
            var query = new { test = "querystring" };
            ModelResp resp = f.post<ModelResp>(data, query);
            Assert.IsNotNull(resp);
            Assert.IsNotNull(resp.origin);
            Assert.IsNotNull(resp.url);
            Assert.IsTrue(resp.args.test == "querystring");
            Assert.IsTrue(resp.headers.test == "headerval");
            Assert.IsTrue(resp.json.test == "json data");
        }

        [TestMethod()]
        public void getReturnModelTest()
        {
            Fetch f = new Fetch("http://httpbin.org/get");
            f.header = new { test = "headerval" };
            var query = new { test = "querystring" };
            ModelResp resp = f.get<ModelResp>(query);
            Assert.IsNotNull(resp);
            Assert.IsNotNull(resp.origin);
            Assert.IsNotNull(resp.url);
            Assert.IsTrue(resp.args.test == "querystring");
            Assert.IsTrue(resp.headers.test == "headerval");
        }

        [TestMethod()]
        public void getReturnStringTest()
        {
            Fetch f = new Fetch("http://httpbin.org/get");
            f.header = new { test = "headerval" };
            var query = new { test = "querystring" };
            string resp = f.get(query);
            Trace.WriteLine(resp);
            Assert.IsNotNull(resp);
            StringAssert.Contains(resp, "headerval");
            StringAssert.Contains(resp, "querystring");
        }

        [TestMethod()]
        public void getBinaryTest()
        {
            Fetch f = new Fetch("https://raw.githubusercontent.com/NuGet/Home/dev/resources/nuget.png");
            byte[] data = f.getBinary();
            Assert.IsNotNull(data);
        }
    }

    class ModelResp
    {
        public string origin { get; set; }
        public string url { get; set; }
        public ModelData args { get; set; }
        public ModelData headers { get; set; }
        public ModelData json { get; set; }
    }
    class ModelData
    {
        public string test { get; set; }
    }
   

}