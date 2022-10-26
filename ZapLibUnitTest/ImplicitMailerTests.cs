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
    public class ImplicitMailerTests
    {
        [TestMethod()]
        public void SendTest()
        {
            /*
            string SMTP_HOST = "smtp.office365.com";
            string MAIL_ACT = "support5@iqs-t.com";
            string MAIL_PWD = "";
            */

            string SMTP_HOST = "smtp.gmail.com";
            string MAIL_ACT = "maryzap123@gmail.com";
            string MAIL_PWD = "";


            int MAIL_PORT = 465;
            bool MAIL_SSL = true;

            ImplicitMailer m = new ImplicitMailer(SMTP_HOST, MAIL_ACT, MAIL_PWD, MAIL_PORT, MAIL_SSL);

            string TO = "zaplin@iqs-t.com,sollin@iqs-t.com";
            string SUBJECT = $"This is a test mail (ZapLib.ImplicitMailer) - {DateTime.Now}";
            string BODY = $"<h1>Test Mail by ZapLib ImplicitMailer</h1>";
            string[] Attachment = new string[] { @"D:\Downloads\delivered.png", @"D:\Downloads\abstract-christmas-tree.zip", @"D:\Downloads\.gitignore" };


            bool result = m.Send(TO, SUBJECT, BODY, attchments: Attachment);
            Trace.WriteLine(m.ErrMsg);
            Assert.IsTrue(result);
        }
    }
}