using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ZapLib.Tests
{
    [TestClass()]
    public class SQLTests
    {
        [TestMethod()]
        public void quickDynamicQueryTest()
        {
            string Host = "192.168.1.190";
            string DBName = "TestFpage";
            string User = "sa";
            string Password = "1qaz@WSX";
            SQL db = new SQL(Host, DBName, User, Password);
            dynamic[] data = db.quickDynamicQuery("select * from entity");
            Trace.WriteLine(JsonConvert.SerializeObject(data));
            Assert.IsNotNull(data);
        }
    }
}