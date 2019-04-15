using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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