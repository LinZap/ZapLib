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
    public class ExpParamTests
    {
        [TestMethod()]
        public void ReplaceSqlTest()
        {
            string paraName = "sid";
            List<string> ExpParams = new List<string>();
            for (int i = 0; i < 10; i++) ExpParams.Add($"@{paraName}{i}");
            string sql1 = "select * from Table where my_sid = @sid_only and sid in(@sid)";
            string sql2 = "select * from Table where my_sid = @sid_only and sid in(@sid    ) and ao_sid = @sid_ao_sid";
            string sql3 = "select * from (select * from object where oid in (@sid)) as c";
            string sql4 = "select * from Table where my_sid = @sid_only and sid in @sid and ao_sid=@sid_list ";

            ExpParam p = new ExpParam(ExpParams);
            Trace.WriteLine(p.ReplaceSql(sql1, paraName, ExpParams));
            Trace.WriteLine(p.ReplaceSql(sql2, paraName, ExpParams));
            Trace.WriteLine(p.ReplaceSql(sql3, paraName, ExpParams));
            Trace.WriteLine(p.ReplaceSql(sql4, paraName, ExpParams));
        }
    }
}