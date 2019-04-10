using System;
using System.Data;

namespace ZapLib
{
    /// <summary>
    /// SQL 專用的 Model 指定資料類型標籤，標記後 prepare stament 將使用指定類型轉換成 SQL 對應類型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SQLTypeAttribute : Attribute
    {
        /// <summary>
        /// SQL 資料類型
        /// </summary>
        public SqlDbType SQLType { get; private set; }
        /// <summary>
        /// 建構子，指定欄位在 SQL 的資料型態
        /// </summary>
        /// <param name="SQLType"></param>
        public SQLTypeAttribute(SqlDbType SQLType)
        {
            this.SQLType = SQLType;
        }
    }
}
