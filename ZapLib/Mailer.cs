using System;
using System.Net;
using System.Net.Mail;

namespace ZapLib
{
    public class Mailer
    {
        private SmtpClient smtp;
        private MailMessage mail;
        private MyLog log;
        private int retry = 1;
        private int MAIL_PORT;
        private string MAIL_PWD;
        private string MAIL_ACT;
        private bool MAIL_SSL;
        private string MAIL_HOST;

        public Mailer(string MAIL_HOST, string MAIL_ACT, string MAIL_PWD, int MAIL_PORT = 587, bool MAIL_SSL = true, int MAIL_RETRY = 1)
        {
            log = new MyLog();
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_ACT = MAIL_ACT;
            this.MAIL_PWD = MAIL_PWD;
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_PORT = MAIL_PORT;
            this.MAIL_SSL = MAIL_SSL;
            retry = MAIL_RETRY;
        }

        public void send(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(MAIL_HOST)) return;
                smtp = new SmtpClient(MAIL_HOST, MAIL_PORT);
                smtp.Credentials = new NetworkCredential(MAIL_ACT, MAIL_PWD);
                smtp.EnableSsl = MAIL_SSL;
                mail = new MailMessage(MAIL_ACT, to, subject, body);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                send_mail();
            }
            catch (Exception e)
            {
                MyLog log = new MyLog();
                log.write(e.ToString());
            }
        }

        protected void send_mail()
        {
            if (retry > 0)
            {
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception e)
                {
                    log.write("can not send mail: " + e.ToString());
                    retry--;
                    send_mail();
                }
            }
        }

    }
}
