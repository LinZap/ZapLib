using ZapLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;
using ZapLib.Utility;

namespace ZapLib.Tests
{
    [TestClass()]
    public class SQLTests
    {
        [TestMethod()]
        public void getErrorMessage()
        {
            /*
              public int Timeout { get; set; } = 15;
        public bool Encrypt { get; set; } = false;
        public string ApplicationIntent { get; set; } = "ReadWrite";
        public bool MultiSubnetFailover { get; set; } = false;
        public bool TrustServerCertificate { get; set; } = false;
             */
            string Host = "10.190.173.190";
            string DBName = "Fpage";
            string User = "sa";
            string Password = "1qaz@WSX";

            SQL db = new SQL(Host, DBName, User, Password);
            db.Timeout = 15;
            db.Encrypt = false;
            db.ApplicationIntent = "ReadWrite";
            db.MultiSubnetFailover = false;
            db.TrustServerCertificate = false;


            object[] o = db.QuickQuery<object>("select * from object where oid>@oid", new { oid = 1 });


            string error = db.GetErrorMessage();
            Trace.WriteLine("------------------------");
            Trace.WriteLine(error);
            Trace.WriteLine("------------------------");
            Assert.IsNotNull(error);
        }


        [TestMethod()]
        public void quickExec()
        {
            string Host = "10.190.173.190";
            string DBName = "Fpage";
            string User = "sa";
            string Password = "1qaz@WSX";

            SQL db = new SQL(Host, DBName, User, Password);

            var input_para = new
            {
                act = "admin",
                passportcode = "123456789"
            };

            var output_para = new
            {
                res = SqlDbType.Int
            };

            ModelOutput result = db.QuickExec<ModelOutput>("xp_checklogin", input_para);

            if (result == null)
                Console.WriteLine(db.GetErrorMessage());
            else
                Console.WriteLine(db.GetErrorMessage());
        }

        [TestMethod()]
        public void detailControl()
        {
            string Host1 = "192.168.1.190";
            string DBName1 = "TestFpage";
            string User1 = "sa";
            string Password1 = "1qaz@WSX";

            string Host2 = "192.168.1.190";
            string DBName2 = "TestFpage";
            string User2 = "sa";
            string Password2 = "1qaz@WSX";

            SQL db1 = new SQL(Host1, DBName1, User1, Password1);
            SQL db2 = new SQL(Host2, DBName2, User2, Password2);

            db1.Connet();
            db2.Connet();

            if (db1.IsConn && db2.IsConn)
            {
                string sql_1 = "select oid from object";
                string sql_2 = "insert into TestTable(oid) values(@oid)";

                SqlDataReader reader1 = db1.Query(sql_1);
                ModelObject[] data1 = db1.fetch<ModelObject>(reader1, false);
                while (data1 != null && data1.Length > 0)
                {
                    int id = data1[0].oid;
                    var para = new
                    {
                        oid = id
                    };
                    SqlDataReader reader2 = db2.Query(sql_2, para);
                    var data2 = db2.fetch<ModelObject>(reader2, false);
                    if (data2 == null)
                        Console.WriteLine("inserted fail");
                    reader2.Close();
                    data1 = db1.fetch<ModelObject>(reader1, false);
                }
                reader1.Close();
            }
            db1.Close();
            db2.Close();

        }

        [TestMethod()]
        public void bulkCopy()
        {

            var dt = new DataTable();
            dt.Columns.Add("oid", typeof(int));

            for (int i = 0; i < 100000; i++)
            {
                var row = dt.NewRow();
                row["oid"] = i;
                dt.Rows.Add(row);
            }
            string Host = "192.168.1.190";
            string DBName = "TestFpage";
            string User = "sa";
            string Password = "1qaz@WSX";
            SQL db = new SQL(Host, DBName, User, Password);

            bool result = db.quickBulkCopy(dt, "dbo.TestTable");

            if (!result)
                Console.WriteLine(db.GetErrorMessage());

        }

        [TestMethod()]
        public void transaction()
        {
            string Host = "10.9.173.190";
            string DBName = "TestFpage";
            string User = "sa";
            string Password = "1qaz@WSX";


            string sql_1 = "insert into TestTable(oid) values(@oid)";
            string sql_2 = "insert2 into TestTable(oid) values(@oid)";

            SQL db = new SQL(Host, DBName, User, Password, true);

            db.Connet();

            if (db.IsConn)
            {
                try
                {
                    var reader = db.Query(sql_1, new { oid = 10 });
                    reader.Close();
                    reader = db.Query(sql_2, new { oid = 20 });
                    reader.Close();
                    db.Tran.Commit();
                }
                catch (Exception e)
                {
                    db.Tran.Rollback();
                    Trace.WriteLine(e.ToString());
                }
            }
            db.Close();
        }

        [TestMethod()]
        public void quickDynamicQuery()
        {
            string Host = "192.168.1.190";
            string DBName = "TestFpage";
            string User = "sa";
            string Password = "1qaz@WSX";
            SQL db = new SQL(Host, DBName, User, Password);
            dynamic[] data = db.QuickDynamicQuery("select * from entity");

            for (int i = 0; i < data.Length; i++)
            {
                Trace.WriteLine((string)data[i].cname);
            }
            Assert.IsNotNull(data);
        }
        [TestMethod()]
        public void quickDynamicQuery2()
        {
            string Host = "192.168.1.190";
            string DBName = "TestFpage";
            string User = "sa";
            string Password = "1qaz@WSX";
            SQL db = new SQL(Host, DBName, User, Password);
            dynamic[] data = db.QuickDynamicQuery("select * from entity");

            for (int i = 0; i < data.Length; i++)
            {
                Trace.WriteLine((string)data[i].cname);
            }
            Assert.IsNotNull(data);
        }




        [TestMethod()]
        public void QuickQueryTest()
        {
            SQL db = new SQL("DriveConnectionString");
            ModelNullAbleEnumObject[] res = db.QuickQuery<ModelNullAbleEnumObject>("select top 1 oid , uuid from object");
            Trace.WriteLine(JsonConvert.SerializeObject(res));
            if (res == null) db.GetErrorMessage();
            Assert.IsNotNull(res);
        }

        [TestMethod()]
        public void BuildconnStringTest()
        {
            SQL db = new SQL();

            string[] testpool = new string[] {
                @"Data Source=10.190.173.134\support6;Min Pool Size=0;Max Pool Size=100;Pooling=true;Initial Catalog=Drive;Persist Security Info=True;User ID=sa;Password=123456",
                @"Data Source=10.190.173.134\support6;Min Pool Size=0;Max Pool Size=100;Pooling=true;Initial Catalog=Drive;Persist Security Info=True;User ID=sa;Password=123456;Connect Timeout=999",
            };

            string[] exps = new string[]{
                ";Connect Timeout=30",
                ";Connect Timeout=999",
            };

            for (int i = 0; i < testpool.Length; i++)
            {
                string cs = testpool[i];
                string res = db.BuildconnString(cs);
                Trace.WriteLine(cs);
                Trace.WriteLine(res);
                Assert.IsTrue(res.Contains(exps[i]));
            }


        }
    }

    enum MyID
    {
        Ok = 0,
        No = 1
    }

    class ModelNullAbleEnumObject
    {
        public MyID? oid { get; set; }
        public string uuid { get; set; }
    }

    class ModelObject
    {
        public int oid { get; set; }
    };

    class ModelOutput
    {
        public int res { get; set; }
    };

    public class ModelBook
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime since { get; set; }
    };


}