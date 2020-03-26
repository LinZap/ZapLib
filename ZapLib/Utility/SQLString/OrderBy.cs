using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZapLib.Utility.SQLString
{
    /// <summary>
    /// SQL 中 Order By 語法處理工具
    /// </summary>
    public static class OrderBy
    {
        /// <summary>
        ///  從 WhiteList 建立的欄位白名單中，檢查排序的 SQL 語法是否合法且皆為指定的欄位，如果排序的 SQL 語法為空則回傳預設值
        /// </summary>
        /// <param name="sort_columns">排序 SQL 語法，例如：colA desc, colB asc, ...</param>
        /// <param name="def_val">預設排序 SQL 語法，預設為空字串</param>
        /// <returns>白名單過濾後的排序 SQL 語法</returns>
        public delegate string CheckSortColumn(string sort_columns, string def_val = "");


        /// <summary>
        /// 建立 SQL 排序語法檢查功能，將回傳一個 CheckSortColumn 方法，可以使用該方法來清理 SQL 排序語法，使 SQL 排序語法都是白名單規定的欄位
        /// </summary>
        /// <param name="whitelist">可供排序的欄位名稱白名單</param>
        /// <returns>清理 SQL 排序語法的方法 CheckSortColumn</returns>
        public static CheckSortColumn WhiteList(params string[] whitelist)
        {
            if (whitelist == null || whitelist.Length < 1) return delegate (string sort_columns, string def_val) { return def_val; };

            HashSet<string> whitelist_set = new HashSet<string>(whitelist.Select(item => item.ToLower()));

            return delegate (string sort_columns, string def_val)
            {
                if (string.IsNullOrWhiteSpace(sort_columns)) return def_val;

                List<string> checked_column_list = new List<string>();
                string[] column_array = sort_columns.ToLower().Split(',');
                foreach (string column in column_array)
                {
                    Match match = new Regex(@"(\w*\.*\w+) *(\w*)", RegexOptions.IgnoreCase).Match(column);
                    if (match.Success)
                    {
                        string column_name = match.Groups[1].Captures[0].Value,
                               order = match.Groups[2].Captures[0].Value;
                        if (whitelist_set.Contains(column_name))
                        {
                            string sort_script = column_name;
                            whitelist_set.Remove(column_name);
                            switch (order)
                            {
                                case "desc":
                                case "asc":
                                    {
                                        sort_script += " " + order;
                                        break;
                                    }
                            }
                            checked_column_list.Add(sort_script);
                        }
                    }
                }
                return string.Join(",", checked_column_list);
            };
        }


    }

}

