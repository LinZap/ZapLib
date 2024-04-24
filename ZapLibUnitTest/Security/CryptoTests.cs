using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
            string exp = "Tom";
            string actual = c.Md5(exp);
            string expected = _Md5(exp);

            Trace.WriteLine(actual);
            Trace.WriteLine(expected);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void Md5Test2()
        {
            Crypto c = new Crypto(Encoding.UTF8);
            string exp = "English is a West Germanic language in the Indo-European language family, whose speakers, called Anglophones, originated in early medieval EnglandEnglish is a West Germanic language in the Indo-European language family, whose speakers, called Anglophones, originated in early medieval EnglandEnglish is a West Germanic language in the Indo-European language family, whose speakers, called Anglophones, originated in early medieval England";
            string actual = c.Md5(exp);
            string expected = _Md5(exp);

            Trace.WriteLine(actual);
            Trace.WriteLine(expected);

            Assert.AreEqual(expected, actual);

        }

            public string _Md5(string content = "")
        {
            Encoding CryptoEncoding = Encoding.UTF8;
            MD5 md5 = MD5.Create();
            byte[] source = CryptoEncoding.GetBytes(content);
            byte[] crypto = md5.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }
    }
    
}