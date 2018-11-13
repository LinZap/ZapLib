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
            f.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            var res = f.post(data,new { test = "123"},file);

            
            Trace.WriteLine(res);
        }


        [TestMethod()]
        public void post2()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("test", "123");
            data.Add("GG", "wefewfhnwoefoew");
            Fetch f = new Fetch("https://httpbin.org/post");
            //f.contentType = "application/x-www-form-urlencoded";
            var res = f.post(data, new { test = "123" });
            Trace.WriteLine(res);
        }
    }
}