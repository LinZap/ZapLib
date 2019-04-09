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
            string[] result =  reg.Exec(contnet);

            foreach(string s in result)
            {
                Console.WriteLine(s);
            }

            Assert.IsNotNull(result);
        }
    }
}