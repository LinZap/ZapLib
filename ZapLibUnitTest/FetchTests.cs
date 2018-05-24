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
            Fetch f = new Fetch("http://localhost:61256/api/attachment");


            //byte[] file = Encoding.UTF8.GetBytes("ewfewfewfewfewfewfwef");
            byte[] file = File.ReadAllBytes("123.txt");

            string res = f.post(new
            {
                file
            });


            Trace.WriteLine(res);
        }
    }
}