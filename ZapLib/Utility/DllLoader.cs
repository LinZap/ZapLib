using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Utility
{
    /// <summary>
    /// DLL 組件載入工具
    /// </summary>
    public static class DllLoader
    {
        /// <summary>
        /// 強制載入指定名稱的 dll 
        /// </summary>
        /// <param name="dllname">dll 檔案完整名稱或路徑</param>
        /// <returns>組件</returns>
        public static Assembly Load(string dllname)
        {
            if (!File.Exists(dllname)) return null;
            using (Stream dllfs = new FileStream(dllname, FileMode.Open))
            {
                byte[] buffer = new byte[(int)dllfs.Length];
                dllfs.Read(buffer, 0, buffer.Length);
                return Assembly.Load(buffer);
            }
        }
    }
}
