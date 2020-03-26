using System;
using System.Diagnostics;
using System.Dynamic;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZapLibUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string mime = MimeMapping.GetMimeMapping("efjewfiewfjew.xls");
            Console.WriteLine("efjewfiewfjew.xls" + " " + mime);

            mime = MimeMapping.GetMimeMapping("we.png");
            Console.WriteLine("we.png" + " " + mime);
            Assert.IsNotNull(mime);
        }


        [TestMethod]
        public void TestMethod2()
        {
            dynamic aa = DateTime.Now;
            dynamic bb = true;
            dynamic cc = new string[] { };
            dynamic dd = new ExpandoObject();
            dd.Name = "ABC";
            dd.Age = 18;


            dynamic d = new
            {
                Name = "ABC",
                Age = 18
            };

            string Name = GetDynamicValue<string>(d, "Name");
            int Age = GetDynamicValue<int>(d, "Age");
            bool IsAdmin = GetDynamicValue<bool>(d, "IsAdmin");

            Trace.WriteLine("Name: " + Name);
            Trace.WriteLine("Age: " + Age);
            Trace.WriteLine("IsAdmin: " + IsAdmin);


        }

        [TestMethod]
        public void TestMethod3()
        {

            string[] arr = null;
            int? len = arr?.Length;

            if (arr?.Length > 0) Trace.WriteLine("大於0");
            else Trace.WriteLine("小於等於0");
        }




            public dynamic GetDynamicValue<T>(dynamic obj, string key)
        {
            return obj.GetType().GetProperty(key)?.GetValue(obj, null) ?? default(T);
        }
    }
}
