using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Tests
{
    [TestClass()]
    public class ConfigTests
    {
        [TestMethod()]
        public void getTest()
        {
            string val1 = Config.get("Test.Key");
            string val2 = Config.get("ewfi0wefwejfwjefjew-9fjw-9efjwefj9ewfj9");
            StringAssert.Contains(val1, "dont't change this value");
            Assert.IsNull(val2);
        }

        [TestMethod()]
        public void getConnectionStringTest()
        {
            string val1 = Config.getConnectionString("Test.ConnectionString");
            string val2 = Config.getConnectionString("ewfi0wefwejfwjefjew-9fjw-9efjwefj9ewfj9");
            StringAssert.Contains(val1, "Data Source=serverName;Initial Catalog=Northwind;Persist Security Info=True;User ID=userName;Password=passwor");
            Assert.IsNull(val2);
        }
    }
}