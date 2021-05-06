using ZapLib.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;

namespace ZapLib.Utility.Tests
{
    [TestClass()]
    public class CastTests
    {

        [TestMethod()]
        public void ToEnum()
        {
            //Trace.WriteLine(Cast.ToEnum(2, IntEnum.One));
            //Trace.WriteLine(Cast.ToEnum("2", IntEnum.One));

            //Trace.WriteLine(Cast.ToEnum(3, IntEnum.One));
            Trace.WriteLine(Cast.ToEnum("GGG", IntEnum.One));

            //Trace.WriteLine(Cast.ToEnum<IntEnum>(3));
            //Trace.WriteLine(Cast.ToEnum<IntEnum>("GGG"));
        }

        [TestMethod()]
        public void ToEnum1()
        {
            Trace.WriteLine(Cast.ToEnum(2, typeof(IntEnum)));
            Trace.WriteLine(Cast.ToEnum("2", typeof(IntEnum)));
            Trace.WriteLine(Cast.ToEnum(3, typeof(IntEnum)));
            Trace.WriteLine(Cast.ToEnum("GGG", typeof(IntEnum)));
        }

        [TestMethod()]
        public void ToTest()
        {
            DateTime d = DateTime.Now;
            Trace.WriteLine(d);

            DateTime? res = Cast.To<DateTime?>(d);
            Trace.WriteLine(res);

            Assert.IsNotNull(res);

        }

        [TestMethod()]
        public void ToTest1()
        {
            DateTime d = DateTime.Now;
            var type = typeof(DateTime?);
            object dd = Cast.To(d, type);
            Trace.WriteLine(dd);
            Assert.IsNotNull(dd);
        }

        [TestMethod()]
        public void ToTest2()
        {
            DateTime? d = DateTime.Now;
            var type = typeof(DateTime);
            object dd = Cast.To(d, type);
            Trace.WriteLine(dd);
            Assert.IsNotNull(dd);
        }
    }

    public enum IntEnum
    {
        One=1,
        Two = 2
    }

    public enum AnonyEnum
    {
        Zero,One,Two
    }

}