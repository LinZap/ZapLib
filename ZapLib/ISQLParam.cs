using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib
{
    /// <summary>
    /// 自訂 SQL Param 處理方式的介面，可以實作這個介面來定義該如何處理 SQL Param
    /// </summary>
    public interface ISQLParam
    {
        /// <summary>
        /// 自定義處理 SQL Param 的方法
        /// </summary>
        /// <param name="cmd">目前正在使用的資料庫指令物件</param>
        /// <param name="name">Param 的名稱</param>
        /// <param name="attr">使用者定義的要強制指定的 SQL 類型標籤</param>
        void CustomParamProcessing(SqlCommand cmd, string name, ISQLTypeAttribute attr);
    }
}
