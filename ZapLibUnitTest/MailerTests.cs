using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Tests
{
    [TestClass()]
    public class MailerTests
    {
        [TestMethod()]
        public void send()
        {
            string SMTP_HOST = "smtp.gmail.com";
            string MAIL_ACT = "scandal0705@gmail.com";
            string MAIL_PWD = "t123936975";
            int MAIL_PORT = 587;
            bool MAIL_SSL = true;

            Mailer m = new Mailer(SMTP_HOST, MAIL_ACT, MAIL_PWD, MAIL_PORT, MAIL_SSL);

            string TO = "scandal0705@gmail.com";
            string SUBJECT = "This is a test mail";
            string BODY = "<h1>Test Mail by ZapLib Mailer</h1>";

            m.Send(TO, SUBJECT, BODY);

            Assert.IsNotNull(m);

        }
    }
}