using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZapLib.Utility;

namespace ZapLib.Tests
{
    [TestClass()]
    public class DynamicObjectTests
    {
        [TestMethod()]
        public void DynamicObjectTest()
        {
            DynamicObject data = new DynamicObject("MyDynamicObject");

            data.CreateProperty("ao_sid", typeof(int), 100);
            data.CreateProperty("aa_name", typeof(string), "Zap");
            data.CreateProperty("is_admin", typeof(bool?), (bool?)null);
            data.CreateProperty("bdate", typeof(DateTime), DateTime.Parse("2020-03-04"));

            object MyDynamicObject = data.CreateObject();

            Dictionary<string, string> ans = new Dictionary<string, string>()
            {
                {"ao_sid" ,  "100"},
                {"aa_name" ,  "Zap"},
                {"is_admin" ,  null},
                {"bdate" ,   DateTime.Parse("2020-03-04").ToString()},
            };

            Mirror.EachMembers<string, object>(MyDynamicObject, (k, v) =>
            {
                Trace.WriteLine(k + " : " + v?.ToString());
                Assert.AreEqual(v?.ToString(), ans[k]);
            });
        }

    }
}