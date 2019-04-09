using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Security
{
    /// <summary>
    /// 平台驗證的中央參數類別
    /// </summary>
    public class Const
    {
        /// <summary>
        /// 平台驗證的金鑰
        /// </summary>
        public static string Key { get; set; } = "Iq123456";
        /// <summary>
        /// 平台驗證的上帝鑰匙 (可以跳過驗證)
        /// </summary>
        public static string GodKey { get; set; } = "nvOcQMfERrASHCIuE797";
    }
}
