using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZapLib
{
    /// <summary>
    /// 正規表達式工具
    /// </summary>
    public class RegExp
    {
        private Regex rgx;

        /// <summary>
        /// 建構子，初始化正規表達式物件
        /// </summary>
        /// <param name="pattern">正規表達式</param>
        public RegExp(string pattern)
        {
            rgx = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 建構子，初始化正規表達式物件
        /// </summary>
        /// <param name="pattern">正規表達式</param>
        /// <param name="options">正規表達式篩選選項</param>
        public RegExp(string pattern, RegexOptions options)
        {
            rgx = new Regex(pattern, options);
        }

        /// <summary>
        /// 執行正規表達式匹配，將所有結果組成陣列回傳
        /// </summary>
        /// <param name="input">欲匹配的字串</param>
        /// <returns>匹配後的結果</returns>
        public string[] Exec(string input)
        {
            List<string> collector = new List<string>();
            MatchCollection matches = rgx.Matches(input);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                foreach (Group g in groups)
                    collector.Add(g.Value);
            }
            return collector.ToArray();
        }
    }
}
