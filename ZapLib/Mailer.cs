using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// SMTP 郵件發送工具
    /// </summary>
    public class Mailer
    {
        /// <summary>
        /// SMTP 連線物件
        /// </summary>
        public SmtpClient smtp {  set; get; }
        /// <summary>
        /// EMAIL 物件
        /// </summary>
        public MimeMessage mail {  set; get; }
        private MyLog log;
        private int retry = 1;
        private int MAIL_PORT;
        private string MAIL_PWD;
        private string MAIL_ACT;
        private bool MAIL_SSL;
        private string MAIL_HOST;
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; private set; }

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
        /// <param name="cc">副本 (不需要可傳 NULL)</param>
        /// <param name="bcc">密件副本 (不需要可傳 NULL)</param>
        public bool Send(string to, string subject, string body, string cc=null, string bcc=null)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(MAIL_HOST))
                {
                    ErrMsg = "MAIL_HOST is Empty";
                    return false;
                }

                using (smtp = new SmtpClient())
                {
                    SecureSocketOptions option = MAIL_SSL? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                    // 連接 Mail Server (郵件伺服器網址, 連接埠, 是否使用 SSL)
                    smtp.Connect(MAIL_HOST, MAIL_PORT, option);
                    smtp.Authenticate(MAIL_ACT, MAIL_PWD);
                    mail = new MimeMessage();
                    mail.Subject = subject;
                    mail.From.Add(MailboxAddress.Parse(MAIL_ACT));
                    mail.To.Add(MailboxAddress.Parse(to));
                    if(!string.IsNullOrWhiteSpace(cc)) mail.Cc.Add(MailboxAddress.Parse(cc));
                    if (!string.IsNullOrWhiteSpace(bcc)) mail.Bcc.Add(MailboxAddress.Parse(bcc));
                    mail.Body = new TextPart(TextFormat.Html) { Text = body };
                    result = send_mail();
                    smtp.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                ErrMsg = e.ToString();
                MyLog log = new MyLog();
                log.SilentMode = Config.Get("SilentMode");
                log.Write(e.ToString());
                return false;
            }
            return result;
        }

        private bool send_mail()
        {
            if (retry > 0)
            {
                try
                {
                    smtp.Send(mail);
                    return true;
                }
                catch (Exception e)
                {
                    ErrMsg = e.ToString();
                    log.Write("can not send mail: " + e.ToString());
                    retry--;
                    send_mail();
                }
            }
            return false;
        }

    }
}
