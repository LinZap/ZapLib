using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// 執行時間紀錄類別
    /// </summary>
    public class LogExecTime
    {
        /// <summary>行為名稱</summary>
        public string Name { get; set; }

        /// <summary>開始時間</summary>
        public DateTime StartTime { get; set; }

        /// <summary>結束時間</summary>
        public DateTime EndTime { get; set; }

        /// <summary>差異時間</summary>
        public TimeSpan DiffTime { get; set; }

        /// <summary>
        /// 要記錄執行時間的行為名稱, New 出這個物件當下就會開始計算時間
        /// </summary>
        /// <param name="name">行為名稱</param>
        public LogExecTime(string name)
        {
            Name = name;
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// 紀錄執行時間，如果 Config 沒有設 LogExecTime=True，則不會記錄
        /// </summary>
        public void Log()
        {
            EndTime = DateTime.Now;
            DiffTime = EndTime.Subtract(StartTime);
            if (Config.Get("LogExecTime")?.ToLower() == "true")
            {
                MyLog log = new MyLog();
                double sec = DiffTime.TotalMilliseconds;
                if (sec == 0)
                    log.Write($"[Log Exec Time] {Name}\r\nTakes {sec} second");
                else
                {
                    decimal spend = (decimal)(sec / 1000.0);
                    log.Write($"[Log Exec Time] {Name}\r\nTakes {decimal.Round(spend, 3)} second");
                }
                   

            }
        }


    }
}
