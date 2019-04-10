using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ZapLib
{
    /// <summary>
    /// Expend SQL Script，將依據封裝的陣列把 SQL prepare stament 參數 @var 拆解成 @var0, @var1, @var2, ...
    /// </summary>
    public class ExpParam : ISQLParam
    {
        private IEnumerable data;

        /// <summary>
        /// 建構子，只允許給予陣列
        /// </summary>   
        public ExpParam(IEnumerable data)
        {
            this.data = data;
        }

        /// <summary>
        /// Expend SQL Script，將依據封裝的陣列把 SQL prepare stament 參數 @var 拆解成 @var0, @var1, @var2, ...
        /// </summary>
        /// <param name="cmd">資料庫指令物件</param>
        /// <param name="name">Param 的名稱</param>
        /// <param name="attr">使用者定義的要強制指定的 SQL 類型標籤</param>
        public void CustomParamProcessing(SqlCommand cmd, string name, ISQLTypeAttribute attr)
        {
            List<string> expandParamNames = new List<string>();
            int idx = 0;

            if (data != null)
                foreach (object ele in data)
                {
                    string new_name = $"@{name}{idx}";
                    var p = cmd.Parameters.AddWithValue(new_name, ele ?? DBNull.Value);
                    if (attr != null) p.SqlDbType = attr.GetSQLType();
                    expandParamNames.Add(new_name);
                    idx++;
                }

            if (idx == 0)
            {
                var p = cmd.Parameters.AddWithValue($"@{name}", DBNull.Value);
                if (attr != null) p.SqlDbType = ((SQLTypeAttribute)attr).GetSQLType();
            }
            else
                cmd.CommandText = cmd.CommandText.Replace($"@{name}", string.Join(",", expandParamNames));
        }

    }
}
