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
        /// 加密類別使用的編碼方式，可自行指定為 UTF8 (預設為 ASCII)
        /// </summary>
        public Encoding CryptoEncoding { get; set; }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="encoding">編碼方式，預設為 ASCII，可自行修改為 UTF8</param>
        public Crypto(Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.ASCII;
            CryptoEncoding = encoding;
        }


        /// <summary>
        /// MD5 雜湊資料
        /// </summary>
        /// <param name="content">資料</param>
        /// <returns>雜湊後的資料</returns>
        public string Md5(string content = "")
        {       
            byte[] source = CryptoEncoding.GetBytes(content);
            byte[] crypto = Utility.MD5.ComputeHash(source);
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
            des.Key = CryptoEncoding.GetBytes(Const.Key);
            des.IV = CryptoEncoding.GetBytes(this.IV);
            byte[] s = CryptoEncoding.GetBytes(content);
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
            des.Key = CryptoEncoding.GetBytes(Const.Key);
            des.IV = CryptoEncoding.GetBytes(iv);
            byte[] s = new byte[content.Length / 2];
            int j = 0;
            for (int i = 0; i < content.Length / 2; i++)
            {
                s[i] = Byte.Parse(content[j].ToString() + content[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                j += 2;
            }
            ICryptoTransform desencrypt = des.CreateDecryptor();
            return CryptoEncoding.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));
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
