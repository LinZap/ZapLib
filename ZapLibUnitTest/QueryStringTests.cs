using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ZapLib.Utility;

namespace ZapLib.Tests
{
    [TestClass()]
    public class QueryStringTests
    {
        [TestMethod()]
        public void Objectify()
        {
            string qs = "name=Tom&age=10&phone=0987654321";
            ModelData data = QueryString.Objectify<ModelData>(qs);
            Console.WriteLine(data.name);
            Console.WriteLine(data.age);
            Console.WriteLine(data.phone);
        }

        public class ModelData
        {
            public string name { get; set; }
            public int age { get; set; }
            public string phone { get; set; }
            public double temp { get; set; }
        }

        [TestMethod()]
        public void Parse()
        {
            ModelData data = new ModelData()
            {
                name = "真的是太扯",
                age = 18,
                phone = "0987654321",
                temp = 0.33333
            };
            string qs = QueryString.Parse(data);
            Console.WriteLine(qs);
        }

    }
}