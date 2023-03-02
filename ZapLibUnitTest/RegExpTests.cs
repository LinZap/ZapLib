using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ZapLib.Tests
{
    [TestClass()]
    public class RegExpTests
    {
        [TestMethod()]
        public void exec()
        {
            string contnet = @"
                1. Name: Tom Age: 10
                2. Name: Tony Age: 8
                3. Name: Sara Age: 12
                4. Name: Eric Age: 11
            ";


            RegExp reg = new RegExp(@"Name: (\w+) Age: (\d+)");
            string[] result = reg.Exec(contnet);

            foreach (string s in result)
            {
                Console.WriteLine(s);
            }

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ExecTest()
        {

            RegExp reg = new RegExp(@"[A-Z]123456789",RegexOptions.None);

            Trace.WriteLine("A123456789");
            string[] res1 = reg.Exec("A123456789");
            Trace.WriteLine(JsonConvert.SerializeObject(res1));

            Trace.WriteLine("a123456789");
            string[] res2 = reg.Exec("a123456789");
            Trace.WriteLine(JsonConvert.SerializeObject(res2));

        }
    }
}