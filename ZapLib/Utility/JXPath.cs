using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZapLib.Utility
{
    /// <summary>
    /// 萬用的 dynamic xpath 取值工具
    /// </summary>
    public static class JXPath
    {

        /// <summary>
        ///  依照 Xpath 從指定的 json 字串取得指定位置的數值，如果路徑設定有誤或無法取得數值則會回傳 null
        /// </summary>
        /// <param name="json">JSON 字串</param>
        /// <param name="xpath">xpath 路徑</param>
        /// <returns>數值</returns>
        public static string GetValue(string json, string xpath)
        {
            dynamic data = JObject.Parse(json);
            return GetValue(data, xpath);
        }

        /// <summary>
        /// 依照 Xpath 從指定的 dynamic 物件取得指定位置的數值，如果路徑設定有誤或無法取得數值則會回傳 null
        /// </summary>
        /// <param name="d">dynamic 物件</param>
        /// <param name="xpath">xpath 路徑</param>
        /// <returns>數值</returns>
        public static string GetValue(dynamic d, string xpath)
        {
            List<(string, int)> plist = ParseXPath(xpath);

            dynamic current = d;

            try
            {
                foreach ((string key, int idx) in plist)
                {
                    current = idx == -1 ? current[key] : current[key][idx];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return current;
        }

        /// <summary>
        /// 將 Xpath 轉換成路徑 List ，依照階層排列，每個階層意義為: (field 名稱 , idx 索引位置)，索引位置為 -1 時，表示直接回傳該值，否則將視為陣列取裡面特定位置的值
        /// </summary>
        /// <param name="path">XPath 字串</param>
        /// <returns>依照階層排列的路徑資料</returns>
        public static List<(string, int)> ParseXPath(string path)
        {
            string[] snippets = path.Split('/');
            List<(string, int)> plist = new List<(string, int)>();

            foreach (string snp in snippets)
            {
                int start = snp.IndexOf('[');
                int end = snp.IndexOf(']');

                string key = start == -1 ? snp : snp.Substring(0, start);
                string str_idx = start == -1 || end == -1 || start >= end ? "-1" : snp.Substring(start + 1, end - start - 1);
                int idx;
                bool res = int.TryParse(str_idx, out idx);
                if (!res) idx = -1;
                plist.Add((key, idx));
            }

            return plist;
        }
    }
}
