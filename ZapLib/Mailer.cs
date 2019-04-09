using System;
using System.Net;
using System.Net.Mail;
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// SMTP 郵件發送工具
    /// </summary>
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

        /// <summary>
        /// 建構子，初始化必要 SMTP 物件
        /// </summary>
        /// <param name="MAIL_HOST">SMTP Server 位置</param>
        /// <param name="MAIL_ACT">登入帳號</param>
        /// <param name="MAIL_PWD">登入密碼</param>
        /// <param name="MAIL_PORT">SMTP Server 埠號</param>
        /// <param name="MAIL_SSL">是否啟用 SSL</param>
        /// <param name="MAIL_RETRY">是否啟用失敗重寄機制，預設為啟用</param>
        public Mailer(string MAIL_HOST, string MAIL_ACT, string MAIL_PWD, int MAIL_PORT = 587, bool MAIL_SSL = true, int MAIL_RETRY = 1)
        {
            log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_ACT = MAIL_ACT;
            this.MAIL_PWD = MAIL_PWD;
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_PORT = MAIL_PORT;
            this.MAIL_SSL = MAIL_SSL;
            retry = MAIL_RETRY;
        }

        /// <summary>
        /// 送出郵件
        /// </summary>
        /// <param name="to">發送到指定 email 位置，多個用逗號隔開</param>
        /// <param name="subject">信件主旨</param>
        /// <param name="body">信件內文</param>
        public void Send(string to, string subject, string body)
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
                log.SilentMode = Config.Get("SilentMode");
                log.Write(e.ToString());
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
                    log.Write("can not send mail: " + e.ToString());
                    retry--;
                    send_mail();
                }
            }
        }

    }
}
