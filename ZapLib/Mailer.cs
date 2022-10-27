using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
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
        /// 郵件的附件列表
        /// </summary>
        public List<MimePart> AttachmentsList { get; private set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; private set; }

        /// <summary>
        /// 使用連線的加密協議
        /// </summary>
        public SecureSocketOptions SecureSocketOption { get; set; }

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
            AttachmentsList = new List<MimePart>();
            log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_ACT = MAIL_ACT;
            this.MAIL_PWD = MAIL_PWD;
            this.MAIL_HOST = MAIL_HOST;
            this.MAIL_PORT = MAIL_PORT;
            this.MAIL_SSL = MAIL_SSL;
            SecureSocketOption = MAIL_SSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
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
        /// <param name="attchments">附加檔案</param>
        public bool Send(string to, string subject, string body, string cc=null, string bcc=null, string[] attchments= null)
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
                    
                    // 連接 Mail Server (郵件伺服器網址, 連接埠, 是否使用 SSL)
                    smtp.Connect(MAIL_HOST, MAIL_PORT, SecureSocketOption);
                    if(!string.IsNullOrWhiteSpace(MAIL_PWD)) smtp.Authenticate(MAIL_ACT, MAIL_PWD);
                    mail = new MimeMessage();
                    mail.Subject = subject;

                    mail.From.AddRange(InternetAddressList.Parse(MAIL_ACT));
                    mail.To.AddRange(InternetAddressList.Parse(to));
                    if(!string.IsNullOrWhiteSpace(cc)) mail.Cc.AddRange(InternetAddressList.Parse(cc));
                    if (!string.IsNullOrWhiteSpace(bcc)) mail.Bcc.AddRange(InternetAddressList.Parse(bcc));

                    Multipart multipart = new Multipart("mixed");
                    multipart.Add(new TextPart(TextFormat.Html) { Text = body });

                    if (attchments != null)
                    {
                        foreach(string p in attchments)
                        {
                            AddAttachments(p);    
                        }
                    }

                    foreach(MimePart p in AttachmentsList)
                    {
                        multipart.Add(p);
                    }
                    

                    mail.Body = multipart;
                    result = send_mail();
                    smtp.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                ErrMsg = e.ToString();
                MyLog log = new MyLog();
                log.SilentMode = Config.Get("SilentMode");
                log.Write($"SMTP Connect Fail, MAIL_HOST={MAIL_HOST}, MAIL_PORT={MAIL_PORT}, SecureSocketOption={SecureSocketOption}, MAIL_ACT={MAIL_ACT}, MAIL_PWD={MAIL_PWD}");
                log.Write(e.ToString());
                return false;
            }
            return result;
        }

        /// <summary>
        /// 新增郵件附加檔案，回傳檔案的 content id
        /// </summary>
        /// <param name="path">檔案實體路徑，如果檔案不存在則回傳 null</param>
        /// <returns>檔案的 content id</returns>
        public string AddAttachments(string path)
        {
            if (!File.Exists(path)) return null;
            string mimeType = MimeMapping.GetMimeMapping(path) ?? "application/octet-stream";
            MimePart attach = new MimePart(mimeType)
            {
                Content = new MimeContent(File.OpenRead(path)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path),
                ContentId = MimeUtils.GenerateMessageId()
        };
          
            AttachmentsList.Add(attach);
            return attach.ContentId;
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
                    log.Write($"SMTP Send Fail, MAIL_HOST={MAIL_HOST}, MAIL_PORT={MAIL_PORT}, SecureSocketOption={SecureSocketOption}, MAIL_ACT={MAIL_ACT}, MAIL_PWD={MAIL_PWD}");
                    log.Write(e.ToString());
                    retry--;
                    send_mail();
                }
            }
            return false;
        }

       
    }
}
