using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ZapLib.Utility.Tests
{
    [TestClass()]
    public class ConfigTests
    {
        [TestMethod()]
        public void GetTest()
        {
            //<add key="aspnet:DontUsePercentUUrlEncoding" value="true" />
            string key = "aspnet:DontUsePercentUUrlEncoding";
            bool val = true;

            string config = Config.Get(key);
            if(config!=null)
            {
                bool.TryParse(config, out bool res);
                Trace.WriteLine(config);
                Assert.AreEqual(val, res);
            }
        }
    }
}