using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Specialized;
using System.Configuration;

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

        [TestMethod()]
        public void GetConnectionStringTest()
        {
            string s = Config.GetConnectionString("DriveConnectionString");
            Assert.IsNotNull(s);
            s = Config.GetConnectionString("Drive2ConnectionString");
            Assert.IsNull(s);
            s = Config.GetConnectionString(null);
            Assert.IsNull(s);
        }

        [TestMethod()]
        public void SetOrAddTest()
        {
            string key = "CurrentTime";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool res = Config.SetOrAdd(key, timestamp);
            Assert.IsTrue(res);
            string actual_timestamp = Config.Get(key);
            Assert.AreEqual(timestamp, actual_timestamp);

            timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            res = Config.SetOrAdd(key, timestamp);
            Assert.IsTrue(res);
            actual_timestamp = Config.Get(key);
            Assert.AreEqual(timestamp, actual_timestamp);
        }

        [TestMethod()]
        public void SetOrAddConnectionStringTest()
        {
            string key = "KB52ConnectionString";
            string connectionstring = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool res = Config.SetOrAddConnectionString(key, connectionstring);
            Assert.IsTrue(res);
            string actual_connectionstring = Config.GetConnectionString(key);
            Assert.AreEqual(connectionstring, actual_connectionstring);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            NameValueCollection collection = Config.Get();
            foreach (string key in collection.AllKeys)
            {

                foreach (string value in collection.GetValues(key))
                {
                    Trace.WriteLine("<" + key + ">" + " : " + value);
                }
            }
            Assert.IsNotNull(collection);
        }

        [TestMethod()]
        public void GetConnectionStringsTest()
        {
            var cscolleciton = Config.GetConnectionStrings();

            foreach (ConnectionStringSettings cs in cscolleciton)
            {
                Trace.WriteLine("<" + cs.Name + ">" + " : " + cs.ConnectionString);
            }
            Assert.IsNotNull(cscolleciton);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            string key = "CurrentTime";
            Config.Delete(key);
            var actual_timestamp = Config.Get(key);
            Assert.IsNull(actual_timestamp);

        }

        [TestMethod()]
        public void DeleteConnectionStringTest()
        {

            string key = "KB52ConnectionString";
            Config.DeleteConnectionString(key);
            var actual_connectionstring = Config.GetConnectionString(key);
            Assert.IsNull(actual_connectionstring);
        }
    }
}