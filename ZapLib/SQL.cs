using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ZapLib
{
    public class SQL
    {
        public bool isConn = false;
        public SqlTransaction tran;
        public SqlCommand cmd;

        private string connString;
        private SqlConnection Conn = null;
        private bool isTran = false;
        private MyLog log;

        public int Timeout { get; set; } = 15;
        public bool Encrypt { get; set; } = false;
        public string ApplicationIntent { get; set; } = "ReadWrite";
        public bool MultiSubnetFailover { get; set; } = false;
        public bool TrustServerCertificate { get; set; } = false;

        /* use system defined */
        public SQL(bool transaction = false)
        {
            string DBName = Config.get("DBName"),
                   DBHost = Config.get("DBHost"),
                   DBAct = Config.get("DBAct"),
                   DBPwd = Config.get("DBPwd"),
                   template = "Server={0};Database={1};User ID={2};Password={3}",
                   basestring = string.Format(template, DBHost, DBName, DBAct, DBPwd);
            isTran = transaction;
            connString = buildconnString(basestring);
            log = new MyLog();
        }

        /* exactlly user defined */
        public SQL(string dbHost, string dbName, string dbAct, string dbPwd, bool transaction = false)
        {
            string template = "Server={0};Database={1};User ID={2};Password={3}",
                   basestring = string.Format(template, dbHost, dbName, dbAct, dbPwd);
            isTran = transaction;
            connString = buildconnString(basestring);
            log = new MyLog();
        }

        public SqlConnection connet()
        {
            try
            {
                Conn = new SqlConnection(connString);
                Conn.Open();

                cmd = new SqlCommand();
                cmd.Connection = Conn;
                isConn = true;

                if (isTran)
                {
                    tran = Conn.BeginTransaction();
                    cmd.Transaction = tran;
                }
            }
            catch (Exception e)
            {
                log.write(e.ToString());
                Conn = null;
            }
            return Conn;
        }

        private string buildconnString(string s)
        {
            s += ";Connect Timeout=" + Timeout;
            s += ";Encrypt" + Encrypt;
            s += ";TrustServerCertificate=" + TrustServerCertificate;
            s += ";ApplicationIntent=" + ApplicationIntent;
            s += ";MultiSubnetFailover=" + MultiSubnetFailover;
            return s;
        }

        /*
           exec normal sql string or procedure
           sql: select @a @b
           params: [{a=1},{b=2}] (Dictionary {key,val})
        */
        public SqlDataReader query(string sql, object param = null)
        {
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            setParaInput(cmd, param);
            return cmd.ExecuteReader();
        }

        /*
            exec stroed procedure 
            not handle error 
            else object (T)
        */
        public T exec<T>(string sql, object param = null, object output = null)
        {
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            setParaInput(cmd, param);
            Dictionary<string, SqlParameter> tmpOutputParams = output == null ? null : setParaOutput(cmd, output);
            cmd.ExecuteNonQuery();
            return getParaOutput<T>(tmpOutputParams);
        }

        /*
            once connect and query and then close connection
            single conn, query and (fetchAll or row)
            @sql 
            @cols
            @isfetchall (default = true)
        */
        public T[] quickQuery<T>(string sql, object param = null, bool isfetchall = true)
        {
            T[] data = null;
            connet();
            if (isConn)
            {
                try
                {
                    SqlDataReader stmt = query(sql, param);
                    if (stmt != null)
                    {
                        data = fetch<T>(stmt, isfetchall);
                        stmt.Close();
                    }
                    if (isTran) tran.Commit();
                }
                catch (Exception e)
                {
                    log.write("SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    log.write(e.ToString());

                    try
                    {
                        if (isTran)
                            tran.Rollback();
                    }
                    catch (Exception x)
                    {
                        log.write("SQL Error: Can not rollback, " + sql + " para:" + JsonConvert.SerializeObject(param));
                        log.write(x.ToString());
                    }
                }
                close();
            }
            return data;
        }


        /*
            exec stored procedure
            params: [{},{}...]
            output: ["",""...]
            Dictionary<output, val>
        */
        public T quickExec<T>(string sql, object param = null, object output = null)
        {
            T obj = default(T);
            connet();
            if (isConn)
            {
                try
                {
                    obj = exec<T>(sql, param, output);
                    if (isTran) tran.Commit();
                }
                catch (Exception e)
                {
                    log.write("SQL Error:" + sql + " para:" + JsonConvert.SerializeObject(param));
                    log.write(e.ToString());
                    try
                    {
                        if (isTran)
                            tran.Rollback();
                    }
                    catch (Exception x)
                    {
                        log.write("SQL Error: Can not rollback, " + sql + " para:" + JsonConvert.SerializeObject(param));
                        log.write(x.ToString());
                    }
                }
                close();
            }
            return obj;
        }

        /*
            fetch one and return T Array 
            if query fail reutrn null
            else return T[] -> otherwise no one row, array length always 1
        */
        public T[] fetch<T>(SqlDataReader r, bool fetchAll = true)
        {
            if (r == null) return null;

            List<T> data = new List<T>();
            T obj = default(T);

            while (r.Read())
            {
                obj = (T)Activator.CreateInstance(typeof(T));
                for (int i = 0; i < r.FieldCount; i++)
                {
                    var value = r.GetValue(i);
                    var prop = obj.GetType().GetProperty(r.GetName(i));
                    if ((prop != null) && prop.CanWrite)
                        prop.SetValue(obj, Convert.IsDBNull(value) ? null : value, null);
                }
                data.Add(obj);
                if (!fetchAll) break;
            }

            return data.ToArray();
        }

        /*
            bind sql params values          
        */
        private void setParaInput(SqlCommand cmd, object param)
        {
            if (param != null)
                foreach (var prop in param.GetType().GetProperties())
                {
                    var value = prop.GetValue(param, null) ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@" + prop.Name, value);
                }
        }

        /*
            set output paras
            return output paras set 
        */
        private Dictionary<string, SqlParameter> setParaOutput(SqlCommand cmd, object output)
        {
            Dictionary<string, SqlParameter> tmpOutputParams = null;
            // add and collect output params
            if (output != null)
            {
                tmpOutputParams = new Dictionary<string, SqlParameter>();
                foreach (var prop in output.GetType().GetProperties())
                {
                    var value = prop.GetValue(output, null);
                    if (value.GetType() != typeof(SqlDbType))
                        log.write("when SQL.setParaOutput: output key " + prop.Name + " is not SqlDbType type");
                    SqlParameter outputIdParam = ((SqlDbType)value == SqlDbType.NVarChar) ? new SqlParameter("@" + prop.Name, (SqlDbType)value, 4000) : new SqlParameter("@" + prop.Name, (SqlDbType)value);
                    outputIdParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputIdParam);
                    tmpOutputParams.Add(prop.Name, outputIdParam);
                }
            }
            return tmpOutputParams;
        }

        /*
            get output paras values 
        */
        private T getParaOutput<T>(Dictionary<string, SqlParameter> tmpOutputParams)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            if (tmpOutputParams != null)
            {
                foreach (var item in tmpOutputParams)
                {
                    string name = item.Key;
                    object value = item.Value.Value;
                    var prop = obj.GetType().GetProperty(name);
                    if ((prop != null) && prop.CanWrite)
                    {
                        if (value.GetType() == typeof(DBNull)) value = null;
                        try
                        {
                            prop.SetValue(obj, value, null);
                        }
                        catch (Exception e)
                        {
                            log.write("can not covert type mapping to Model: " + e.ToString());
                        }
                    }
                }
            }
            return obj;
        }

        public void close()
        {
            if (Conn != null)
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
        }

    }
}
