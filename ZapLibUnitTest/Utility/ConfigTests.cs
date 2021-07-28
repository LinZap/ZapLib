using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

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
            if (config != null)
            {
                bool.TryParse(config, out bool res);
                Trace.WriteLine(config);
                Assert.AreEqual(val, res);
            }
        }

        [TestMethod()]
        public void RefreshTest()
        {
            string storage = Config.Get("Storage");

            Trace.WriteLine("storage = " + storage);

            Trace.WriteLine("wait 10 sec...");

            Thread.Sleep(10000);

            Trace.WriteLine("done!");

            string new_storage_nofresh = Config.Get("Storage");

            Trace.WriteLine("new_storage_nofresh = " + new_storage_nofresh);

            Config.Refresh();
            string new_storage_fresh = Config.Get("Storage");

            Trace.WriteLine("new_storage_nfresh = " + new_storage_fresh);


            Assert.AreEqual(storage, new_storage_nofresh);
            Assert.AreNotEqual(storage, new_storage_fresh);
        }
    }
}