using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using ZapLib;

namespace ZapLib.Tests
{
    [TestClass()]
    public class OracleSQLTests
    {
        private const string ConnectionString =
            "User Id=zaplib;Password=zaplib123;Data Source=10.190.173.129:1521/FREEPDB1";

        public class HR_Row
        {
            public string EMPN { get; set; }
            public string NAME { get; set; }
            public string UNITCD { get; set; }
            public string CUNIT { get; set; }
            public string EUNIT { get; set; }
            public string UNITCD1 { get; set; }
            public string CUNIT1 { get; set; }
            public string EUNIT1 { get; set; }
            public string UNITCD2 { get; set; }
            public string CUNIT2 { get; set; }
            public string EUNIT2 { get; set; }
            public string UNITCD3 { get; set; }
            public string CUNIT3 { get; set; }
            public string EUNIT3 { get; set; }
            public int? UNITLVL { get; set; }
            public string EMAIL { get; set; }
        }

        [TestMethod()]
        public void QuickDynamicQuery_HR_VIEW_Count()
        {
            var db = new OracleSQL(ConnectionString);
            dynamic[] rows = db.QuickDynamicQuery("SELECT * FROM HR_VIEW");

            string error = db.GetErrorMessage();
            Trace.WriteLine("---- error ----");
            Trace.WriteLine(error);

            Assert.IsNotNull(rows, "QuickDynamicQuery returned null. Error: " + error);
            Assert.AreEqual(50, rows.Length, "Expected 50 rows in HR_VIEW");
        }

        [TestMethod()]
        public void QuickQuery_HR_VIEW_TypedMapping()
        {
            var db = new OracleSQL(ConnectionString);
            HR_Row[] rows = db.QuickQuery<HR_Row>("SELECT * FROM HR_VIEW ORDER BY EMPN");

            string error = db.GetErrorMessage();
            Trace.WriteLine("---- error ----");
            Trace.WriteLine(error);
            Trace.WriteLine("---- first row ----");
            Trace.WriteLine(JsonConvert.SerializeObject(rows?.FirstOrDefault()));

            Assert.IsNotNull(rows, "QuickQuery returned null. Error: " + error);
            Assert.AreEqual(50, rows.Length, "Expected 50 rows in HR_VIEW");

            HR_Row first = rows.First();
            Assert.AreEqual("625871", first.EMPN);
            Assert.AreEqual("陳○威", first.NAME);
            Assert.AreEqual("資訊系統部", first.CUNIT);
            Assert.AreEqual("IT SYSTEMS DEPARTMENT", first.EUNIT);
            Assert.AreEqual(2, first.UNITLVL);
            Assert.AreEqual("aji0y6dp@china-airlines.com", first.EMAIL);
            Assert.IsNull(first.UNITCD3, "UNITCD3 should be NULL for this row");
        }

        [TestMethod()]
        public void QuickQuery_HR_VIEW_WithParam()
        {
            var db = new OracleSQL(ConnectionString);
            HR_Row[] rows = db.QuickQuery<HR_Row>(
                "SELECT * FROM HR_VIEW WHERE EMPN = @empn",
                new { empn = "625871" });

            string error = db.GetErrorMessage();
            Trace.WriteLine("---- error ----");
            Trace.WriteLine(error);

            Assert.IsNotNull(rows, "QuickQuery returned null. Error: " + error);
            Assert.AreEqual(1, rows.Length);
            Assert.AreEqual("陳○威", rows[0].NAME);
        }
    }
}
