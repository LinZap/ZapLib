using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib
{
    /// <summary>
    /// SQL 專用的 Model 指定資料類型標籤介面，標記後 prepare stament 將使用指定類型轉換成 SQL 對應類型
    /// </summary>
    public interface ISQLTypeAttribute
    {
        /// <summary>
        /// 取得指定轉換成 SQL 對應類型
        /// </summary>
        /// <returns>Database 資料類型</returns>
        SqlDbType GetSQLType();

        /// <summary>
        /// 存取 SQL 對應類型的資料長度
        /// </summary>
        int Size { get; set; }
    }
}
