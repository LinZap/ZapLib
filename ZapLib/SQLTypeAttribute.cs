using System;
using System.Data;

namespace ZapLib
{
    /// <summary>
    /// SQL 專用的 Model 指定資料類型標籤，標記後 prepare stament 將使用指定類型轉換成 SQL 對應類型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SQLTypeAttribute : Attribute, ISQLTypeAttribute
    {
        private SqlDbType SQLType;
        /// <summary>
        /// 建構子，指定欄位在 SQL 的資料型態
        /// </summary>
        /// <param name="SQLType">資料庫資料型態</param>
        public SQLTypeAttribute(SqlDbType SQLType)
        {
            this.SQLType = SQLType;
        }

        /// <summary>
        /// 建構子，指定欄位在 SQL 的資料型態與長度
        /// </summary>
        /// <param name="SQLType">資料庫資料型態</param>
        /// <param name="size">資料長度</param>
        public SQLTypeAttribute(SqlDbType SQLType, int size)
        {
            this.SQLType = SQLType;
            Size = size;
        }

        /// <summary>
        /// 存取 SQL 對應類型的資料長度
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 取得指定轉換成 SQL 對應類型
        /// </summary>
        /// <returns>Database 資料類型</returns>
        public SqlDbType GetSQLType() => SQLType;
    }
}
