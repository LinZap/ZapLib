using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZapLib.Security
{
    public class Crypto
    {
        public string iv { get; set; }

        public string Md5(string content = "")
        {
            MD5 md5 = MD5.Create();
            byte[] source = Encoding.Default.GetBytes(content);
            byte[] crypto = md5.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }

        public string DESEncryption(string content, string iv = null)
        {
            if (iv != null) this.iv = iv;
            if (this.iv == null) this.iv = randomString(Const.Key.Length);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.ASCII.GetBytes(Const.Key);
            des.IV = Encoding.ASCII.GetBytes(this.iv);
            byte[] s = Encoding.ASCII.GetBytes(content);
            ICryptoTransform desencrypt = des.CreateEncryptor();
            return BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty);
        }

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

        public string randomString(int len)
        {
            Random random = new Random();
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
