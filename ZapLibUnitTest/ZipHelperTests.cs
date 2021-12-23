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
    public class ZipHelperTests
    {
        [TestMethod()]
        public void ZipTest()
        {
            ZipHelper z = new ZipHelper("TestQQ", @"D:\Storage");
            z.AddFile("", @"D:\Storage\123.txt", "readme.txt");
            z.AddFile("/img", @"D:\Storage\123.txt", "pig.txt");
            z.AddFile("/img/favorite", @"D:\Storage\456.txt", "apple.txt");
            z.AddFile("/music", @"D:\Storage\789.txt", "/");
            z.AddFile("/music/good", @"D:\Storage\AAA.txt", "good.txt");
            foreach ((string foldername, string filedist, string filename) in z.FileList)
            {
                Trace.WriteLine(foldername);
                Trace.WriteLine(filedist);
                Trace.WriteLine(filename);
                Trace.WriteLine("-------------------------------------------");
            }
            bool res = z.Zip();


            if (!res)
                foreach (string log in z.ErrorLogs)
                {
                    Trace.WriteLine(log);
                    Trace.WriteLine("-------------------------------------------");
                }
            else
                Trace.WriteLine("zip file: " + z.ZipDist);

            Assert.IsTrue(res);
        }
    }
}