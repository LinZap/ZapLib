using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web.Http;

namespace ZapLib.Utility.Tests
{
    [TestClass()]
    public class MirrorTests
    {
        [TestMethod()]
        public void GetClassesTest()
        {
            foreach(Type assm in Mirror.GetClasses<BBB>(true))
            {
                Trace.WriteLine(assm.Name);
                Trace.WriteLine(assm.GetType().Name);
                Trace.WriteLine("");
            }
        }
    }

    public class AAA{}
    public class BBB:AAA { }

 
}