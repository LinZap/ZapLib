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
            Fetch f = new Fetch("https://httpbin.org/get");
            f.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            f.header = new { };
            var res = f.get(null);

            Trace.WriteLine(res);
        }
    }
}