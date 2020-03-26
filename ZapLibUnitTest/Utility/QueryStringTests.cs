using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ZapLib.Utility.Tests
{
    [TestClass()]
    public class QueryStringTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string res = QueryString.Parse(new { q = "韓",q2="123" });
            Trace.WriteLine(res);
        }
    }
}