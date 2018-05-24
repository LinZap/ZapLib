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
    public class SQLTests
    {
        [TestMethod()]
        public void getErrorMessage()
        {
            SQL db = new SQL("192.168.1.190", "Fpage", "fpage", "Iq12345667890");
            object[] o = db.quickQuery<object>("select * from class");
            string error = db.getErrorMessage();
            Trace.WriteLine("------------------------");
            Trace.WriteLine(error);
            Trace.WriteLine("------------------------");
            Assert.IsNotNull(error);
        }


       
    }
}