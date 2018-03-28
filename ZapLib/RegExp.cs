using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZapLib
{
    public class RegExp
    {
        private Regex rgx;

        public RegExp(string pattern)
        {
            rgx = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public string[] exec(string input)
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
