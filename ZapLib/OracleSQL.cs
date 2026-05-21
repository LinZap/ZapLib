using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// Oracle 連線查詢輔助工具，使用方式比照 <see cref="SQL"/>。
    /// 內部使用 Oracle.ManagedDataAccess，純託管、無須安裝 Oracle Client。
    /// </summary>
    public class OracleSQL
    {
        /// <summary>
        /// 是否已經與資料庫連線成功
        /// </summary>
        public bool IsConn { get; private set; } = false;

        /// <summary>
        /// 目前使用的資料庫交易物件（此版本預留，未啟用）
        /// </summary>
        public OracleTransaction Tran { get; private set; }

        /// <summary>
        /// 目前使用的資料庫命令物件
        /// </summary>
        public OracleCommand Cmd { get; private set; }

        /// <summary>
        /// 資料庫連線逾時秒數，預設 30 秒
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// 追蹤碼，同一個追蹤碼代表同一個 OracleSQL 物件
        /// </summary>
        public string TraceCode { get; private set; }

        private string connString;
        private OracleConnection Conn = null;
        private MyLog log;
        private List<string> errormessage;

        private static readonly Regex ParamRegex = new Regex(@"@(\w+)", RegexOptions.Compiled);

        /// <summary>
        /// 初始化 OracleSQL 連線物件，嘗試從 .config 中抓取指定名稱的連線字串或直接給予連線字串進行連線
        /// </summary>
        /// <param name="connectionString">指定的連線字串名稱 或 連線字串</param>
        public OracleSQL(string connectionString)
        {
            connString = Config.GetConnectionString(connectionString) ?? connectionString;
            GeneralProcessing();
        }

        private void GeneralProcessing()
        {
            log = new MyLog();
            log.SilentMode = Config.Get("SilentMode");
            errormessage = new List<string>();
            TraceCode = Guid.NewGuid().ToString();

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connString;
            if (builder.ContainsKey("Connect Timeout"))
            {
                int.TryParse(builder["Connect Timeout"]?.ToString(), out int conn_str_timeout);
                Timeout = conn_str_timeout;
            }
        }

        /// <summary>
        /// 取得所有資料庫的錯誤訊息
        /// </summary>
        public string GetErrorMessage() => string.Join("\n", errormessage);

        /// <summary>
        /// 取得主要的資料庫連線物件
        /// </summary>
        public OracleConnection GetConnection() => Conn;

        /// <summary>
        /// 手動連線資料庫，可以使用 IsConn 來確認是否連線成功
        /// </summary>
        [Obsolete("方法已重新命名為 Connect，原拼字錯誤的版本將於下一個 major 版本移除")]
        public void Connet() => Connect();

        /// <summary>
        /// 手動連線資料庫，可以使用 IsConn 來確認是否連線成功
        /// </summary>
        public void Connect()
        {
            LogExecTime lextime = new LogExecTime($"DB Connection time\r\nTraceCode: {TraceCode}");
            try
            {
                Conn = new OracleConnection(connString);
                Conn.Open();
                Cmd = new OracleCommand();
                Cmd.Connection = Conn;
                Cmd.CommandTimeout = Timeout;
                Cmd.BindByName = true;
                IsConn = true;
            }
            catch (Exception e)
            {
                log.Write(e.ToString());
                errormessage.Add(e.ToString());
                Conn = null;
                IsConn = false;
            }
            lextime.Log();
        }

        /// <summary>
        /// 手動執行查詢命令，需自行控制可能發生的錯誤
        /// </summary>
        /// <param name="sql">查詢語法（參數可使用 @name 寫法，將自動轉為 Oracle 的 :name）</param>
        /// <param name="param">語法中的參數化資料</param>
        /// <returns>OracleDataReader 物件</returns>
        public OracleDataReader Query(string sql, object param = null)
        {
            LogExecTime lextime2 = new LogExecTime($"Exec sql: {sql}\r\nParam: {JsonConvert.SerializeObject(param)}\r\nTraceCode: {TraceCode}");
            string oracleSql = ConvertParamPrefix(sql);
            Cmd.CommandText = oracleSql;
            Cmd.CommandType = CommandType.Text;
            Cmd.Parameters.Clear();
            SetParaInput(Cmd, param);
            OracleDataReader rd = Cmd.ExecuteReader();
            lextime2.Log();
            return rd;
        }

        /// <summary>
        /// 自動開啟連線並執行查詢語法，執行完畢後自動關閉連線
        /// </summary>
        /// <typeparam name="T">將返回資料綁定到指定類型</typeparam>
        /// <param name="sql">查詢語法</param>
        /// <param name="param">語法中的參數化資料</param>
        /// <param name="isfetchall">是否取出所有資料，預設為 true，否則只取出 1 筆</param>
        /// <returns>綁定查詢語法輸出表格的資料模型陣列</returns>
        public T[] QuickQuery<T>(string sql, object param = null, bool isfetchall = true)
        {
            T[] data = null;
            Connect();
            if (IsConn)
            {
                try
                {
                    OracleDataReader stmt = Query(sql, param);
                    if (stmt != null)
                    {
                        data = fetch<T>(stmt, isfetchall);
                        stmt.Close();
                    }
                }
                catch (Exception e)
                {
                    log.Write("Oracle SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    errormessage.Add("Oracle SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    errormessage.Add(e.ToString());
                    log.Write(e.ToString());
                }
                Close();
            }
            return data;
        }

        /// <summary>
        /// 自動開啟連線並執行查詢語法，執行完畢後自動關閉連線
        /// </summary>
        /// <param name="sql">查詢語法</param>
        /// <param name="param">語法中的參數化資料</param>
        /// <param name="isfetchall">是否取出所有資料，預設為 true，否則只取出 1 筆</param>
        /// <returns>綁定查詢語法輸出表格的動態資料陣列</returns>
        public dynamic[] QuickDynamicQuery(string sql, object param = null, bool isfetchall = true)
        {
            dynamic[] data = null;
            Connect();
            if (IsConn)
            {
                try
                {
                    OracleDataReader stmt = Query(sql, param);
                    if (stmt != null)
                    {
                        data = dynamicFetch(stmt, isfetchall);
                        stmt.Close();
                    }
                }
                catch (Exception e)
                {
                    log.Write("Oracle SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    errormessage.Add("Oracle SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    errormessage.Add(e.ToString());
                    log.Write(e.ToString());
                }
                Close();
            }
            return data;
        }

        /// <summary>
        /// 手動執行取出查詢結果的資料，並綁定到指定的資料模型中
        /// </summary>
        /// <typeparam name="T">將資料綁定到指定類型</typeparam>
        /// <param name="r">資料讀取物件</param>
        /// <param name="fetchAll">是否取出所有資料，預設為 true，否則只取出 1 筆</param>
        /// <returns>綁定資料的模型陣列</returns>
        public T[] fetch<T>(OracleDataReader r, bool fetchAll = true)
        {
            if (r == null) return null;
            List<T> data = new List<T>();
            T obj = default(T);
            LogExecTime lxt = new LogExecTime($"Oracle Collect Data (fetch<T>)\r\nTraceCode: {TraceCode}");
            while (r.Read())
            {
                obj = (T)Activator.CreateInstance(typeof(T));
                for (int i = 0; i < r.FieldCount; i++)
                {
                    object value = r.GetValue(i);
                    var prop = obj.GetType().GetProperty(r.GetName(i), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if ((prop != null) && prop.CanWrite)
                    {
                        value = Convert.IsDBNull(value) ? null : value;
                        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            value = Cast.To(value, prop.PropertyType);
                        }
                        prop.SetValue(obj, value, null);
                    }
                }
                data.Add(obj);
                if (!fetchAll) break;
            }
            T[] resarray = data.ToArray();
            lxt.Log();
            return resarray;
        }

        /// <summary>
        /// 手動執行取出查詢結果的資料，並綁定到動態資料中
        /// </summary>
        /// <param name="r">資料讀取物件</param>
        /// <param name="fetchAll">是否取出所有資料，預設為 true，否則只取出 1 筆</param>
        /// <returns>綁定資料的動態資料陣列</returns>
        public dynamic[] dynamicFetch(OracleDataReader r, bool fetchAll = true)
        {
            if (r == null) return null;
            List<dynamic> data = new List<dynamic>();
            LogExecTime lxt = new LogExecTime($"Oracle Collect Data (dynamicFetch)\r\nTraceCode: {TraceCode}");
            while (r.Read())
            {
                IDictionary<string, object> dict = new ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < r.FieldCount; i++)
                {
                    var value = r.GetValue(i);
                    var key = r.GetName(i);
                    dict[key] = Convert.IsDBNull(value) ? null : value;
                }
                data.Add(dict);
                if (!fetchAll) break;
            }
            dynamic[] resdata = data.ToArray();
            lxt.Log();
            return resdata;
        }

        /// <summary>
        /// 將 SQL 語法中的 @name 參數改寫為 Oracle 的 :name
        /// </summary>
        private static string ConvertParamPrefix(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return sql;
            return ParamRegex.Replace(sql, ":$1");
        }

        /// <summary>
        /// 綁定參數值到 OracleCommand（依屬性名稱對應 :name）
        /// </summary>
        private void SetParaInput(OracleCommand cmd, object param)
        {
            if (param == null) return;
            foreach (var prop in param.GetType().GetProperties())
            {
                var value = prop.GetValue(param, null) ?? DBNull.Value;
                cmd.Parameters.Add(new OracleParameter(prop.Name, value));
            }
        }

        /// <summary>
        /// 手動關閉資料庫連線
        /// </summary>
        public void Close()
        {
            if (Conn != null)
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
        }
    }
}
