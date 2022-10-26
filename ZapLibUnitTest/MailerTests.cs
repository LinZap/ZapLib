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
    public class MailerTests
    {
        [TestMethod()]
        public void send()
        {
            string SMTP_HOST = "smtp.office365.com";
            string MAIL_ACT = "support5@iqs-t.com";
            string MAIL_PWD = "IQChina888";
            int MAIL_PORT = 587;
            bool MAIL_SSL = false;

            Mailer m = new Mailer(SMTP_HOST, MAIL_ACT, MAIL_PWD, MAIL_PORT, MAIL_SSL);

            string TO = "zaplin@iqs-t.com,sollin@iqs-t.com,brucewang@iqs-t.com";
            string SUBJECT = "This is a test mail (ZapLib.Mailer)";
            string BODY = "<h1>Test Mail by ZapLib Mailer</h1>";
            string[] Attachment = new string[] { @"D:\Downloads\delivered.png", @"D:\Downloads\abstract-christmas-tree.zip", @"D:\Downloads\.gitignore" };
            string cid = m.AddAttachments(@"D:\Downloads\user.png");
            Trace.WriteLine("cid:" + cid);
            foreach (var a in m.AttachmentsList)
            {
                BODY += $"<p>圖片在這: <img src=\"cid:{cid}\" ></p>";
            }
            bool result = m.Send(TO, SUBJECT, BODY, attchments: Attachment);
            Trace.WriteLine(m.ErrMsg);
            Assert.IsTrue(result);

        }
    }
}