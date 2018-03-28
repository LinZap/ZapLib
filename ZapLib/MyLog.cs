using System;
using System.IO;

namespace ZapLib
{
    public class MyLog
    {
        public void write(string msg)
        {
            string logfile = Config.get("Storage");
            if (logfile == null) return;
            DateTime now = DateTime.Now;
            string time = now.ToString("HH:mm:ss"),
                   date = now.ToString("yyyyMMdd"),
                   path = string.Format(@"{0}\{1}.txt", logfile, date),
                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine;
            File.AppendAllText(path, txt);
        }
    }
}
