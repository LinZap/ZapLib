using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZapLib.Utility;
using AegisImplicitMail;
using System.ComponentModel;

namespace ZapLib
{
    /// <summary>
    /// 使用隱式 SSL 加密連線協議的 SMTP 寄信工具
    /// </summary>
    public class ImplicitMailer
    {

        private MyLog log;
        private int retry = 1;
        private int MAIL_PORT;
        private string MAIL_PWD;
        private string MAIL_ACT;
        private bool MAIL_SSL;
        private string MAIL_HOST;

        /// <summary>
        /// 郵件本體物件
        /// </summary>
        public MimeMailMessage MimeMailMessage { get; private set; }

        /// <summary>
        /// 隱式SSL SMTPS 寄信物件
        /// </summary>
        public MimeMailer MimeMailer { get; private set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; private set; }

        /// <summary>
        /// 使用連線的加密協議
        /// </summary>
        public SslMode SecureSocketOption { get; set; }

        /// <summary>
        /// 登入驗證類型，預設 Base64 
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// 建構子，初始化必要 SMTP 物件
        /// </summary>
        /// <param name="MAIL_HOST">SMTP Server 位置</param>
        /// <param name="MAIL_ACT">登入帳號</param>
        /// <param name="MAIL_PWD">登入密碼</param>
        /// <param name="MAIL_PORT">隱式 SSL 加密 SMTP Server 埠號，預設 465</param>
        /// <param name="MAIL_SSL">是否啟用 SSL (true: Ssl , false: auto )</param>
        /// <param name="MAIL_RETRY">是否啟用失敗重寄機制，預設為啟用</param>
        public ImplicitMailer(string MAIL_HOST, string MAIL_ACT, string MAIL_PWD, int MAIL_PORT = 465, bool MAIL_SSL = true, int MAIL_RETRY = 1)
        {
            log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_ACT = MAIL_ACT;
            this.MAIL_PWD = MAIL_PWD;
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_PORT = MAIL_PORT;
            this.MAIL_SSL = MAIL_SSL;
            SecureSocketOption = MAIL_SSL ? SslMode.Ssl : SslMode.Auto;
            AuthenticationType = AuthenticationType.Base64;
            MimeMailMessage = new MimeMailMessage();
            MimeMailer = new MimeMailer(MAIL_HOST, MAIL_PORT);
            retry = MAIL_RETRY;
            ErrMsg = "";
        }

        /// <summary>
        /// 送出郵件
        /// </summary>
        /// <param name="to">發送到指定 email 位置，多個用逗號隔開</param>
        /// <param name="subject">信件主旨</param>
        /// <param name="body">信件內文</param>
        /// <param name="cc">副本 (不需要可傳 NULL)</param>
        /// <param name="bcc">密件副本 (不需要可傳 NULL)</param>
        public bool Send(string to, string subject, string body, string cc = null, string bcc = null)
        {

            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(MAIL_HOST))
                {
                    ErrMsg += "MAIL_HOST is Empty";
                    return false;
                }

                MimeMailMessage.From = new MimeMailAddress(MAIL_ACT);

                if (!string.IsNullOrWhiteSpace(to))
                {
                    foreach (var m in to.Split(','))
                    {
                        MimeMailMessage.To.Add(m);
                    }
                }

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    foreach (var m in cc.Split(','))
                    {
                        MimeMailMessage.CC.Add(m);
                    }
                }

                if (!string.IsNullOrWhiteSpace(bcc))
                {
                    foreach (var m in bcc.Split(','))
                    {
                        MimeMailMessage.Bcc.Add(m);
                    }
                }

                MimeMailMessage.Subject = subject;
                MimeMailMessage.IsBodyHtml = true;
                MimeMailMessage.Body = body;

                MimeMailer.User = MAIL_ACT;
                MimeMailer.Password = MAIL_PWD;
                MimeMailer.SslType = SecureSocketOption;
                MimeMailer.AuthenticationMode = AuthenticationType;
                MimeMailer.SendCompleted += compEvent;

                MimeMailer.SendMail(MimeMailMessage);

                result = true;
            }
            catch (Exception e)
            {
                ErrMsg += e.ToString();
                log = new MyLog();
                log.Write($"Implicit Mailer Connect Fail, MAIL_HOST={MAIL_HOST}, MAIL_PORT={MAIL_PORT}, SecureSocketOption={SecureSocketOption}, MAIL_ACT={MAIL_ACT}, MAIL_PWD={MAIL_PWD}");
                log.Write(e.ToString());
                return false;
            }

            return result;
        }


        //Call back function
        private void compEvent(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.Write($"Implicit Mailer Error: { e.Error.Message}");
                ErrMsg += e.Error.Message;
            }
        }

    }
}
