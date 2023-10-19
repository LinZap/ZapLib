using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ZapLib.Security.Tests
{
    [TestClass()]
    public class CryptoTests
    {
        [TestMethod()]
        public void DESEncryptionTest()
        {
            Crypto c = new Crypto(Encoding.UTF8);
            string exp = "你好我是大衛";
            string s = c.DESEncryption(exp);
            Trace.WriteLine(s);
            string ds = c.DESDecryption(s, c.IV);
            Trace.WriteLine(ds);
            Assert.AreEqual(exp, ds);
        }

        [TestMethod()]
        public void Md5Test()
        {
            Crypto c = new Crypto(Encoding.UTF8);
            string exp = "你好我是大衛";
            string s = c.Md5(exp);
            Trace.WriteLine(s);

            string exp2 = "你好我是大衛";
            string s2 = c.Md5(exp2);
            Trace.WriteLine(s2);
        }
    }
    
}