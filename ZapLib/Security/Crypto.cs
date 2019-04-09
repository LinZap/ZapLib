using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZapLib.Security
{
    /// <summary>
    /// 訊息加解密輔助工具
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// DES 加密初始化向量
        /// </summary>
        public string IV { get; set; }

        /// <summary>
        /// MD5 雜湊資料
        /// </summary>
        /// <param name="content">資料</param>
        /// <returns>雜湊後的資料</returns>
        public string Md5(string content = "")
        {
            MD5 md5 = MD5.Create();
            byte[] source = Encoding.Default.GetBytes(content);
            byte[] crypto = md5.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }

        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="content">原始內容</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>加密後的內容</returns>
        public string DESEncryption(string content, string iv = null)
        {
            if (iv != null) this.IV = iv;
            if (this.IV == null) this.IV = RandomString(Const.Key.Length);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.ASCII.GetBytes(Const.Key);
            des.IV = Encoding.ASCII.GetBytes(this.IV);
            byte[] s = Encoding.ASCII.GetBytes(content);
            ICryptoTransform desencrypt = des.CreateEncryptor();
            return BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty);
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="content">加密後的內容</param>
        /// <param name="iv">初始化向量</param>
        /// <returns>解密後的內容</returns>
        public string DESDecryption(string content, string iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.ASCII.GetBytes(Const.Key);
            des.IV = Encoding.ASCII.GetBytes(iv);
            byte[] s = new byte[content.Length / 2];
            int j = 0;
            for (int i = 0; i < content.Length / 2; i++)
            {
                s[i] = Byte.Parse(content[j].ToString() + content[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                j += 2;
            }
            ICryptoTransform desencrypt = des.CreateDecryptor();
            return Encoding.ASCII.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));
        }

        /// <summary>
        /// 產生指定長度的亂數字串 [A-Za-z0-9]
        /// </summary>
        /// <param name="len">指定長度</param>
        /// <returns>亂數字串</returns>
        public string RandomString(int len)
        {
            Random random = new Random();
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
