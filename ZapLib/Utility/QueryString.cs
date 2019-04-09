using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZapLib.Utility
{
    /// <summary>
    /// 萬用的查詢字串輔助工具
    /// </summary>
    public static class QueryString
    {
        /// <summary>
        /// 反序列化查詢字串，依照 key value 指定到指定型態的物件中
        /// </summary>
        /// <typeparam name="T">指定物件類型</typeparam>
        /// <param name="qs">查詢字串</param>
        /// <returns>特定型態的物件中</returns>
        public static T Objectify<T>(string qs)
        {
            qs = WebUtility.UrlDecode(qs ?? string.Empty);
            var query = HttpUtility.ParseQueryString(qs);
            try
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                foreach (string key in query)
                    Mirror.AssignValue(ref obj, key, query[key]);
                return obj;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 序列化查詢字串，依照指定的物件成員名稱與數值組合成查詢字串
        /// </summary>
        /// <param name="data">指定的物件</param>
        /// <returns>查詢字串</returns>
        public static string Parse(object data)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            Mirror.EachMembers(data, (string key, string val) =>
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(val)) return;
                query[key] = val;
            });
            return query.ToString();
        }
    }
}
