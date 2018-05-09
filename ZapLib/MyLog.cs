using System;
using System.IO;

namespace ZapLib
{
    public class MyLog
    {
        public string path { get; set; }
        public string name { get; set; }
        public void write(string msg)
        {
            string logfile = this.path != null ? this.path : Config.get("Storage");
            if (logfile == null) return;
            DateTime now = DateTime.Now;
            string name = this.name != null ? this.name : now.ToString("yyyyMMdd"),
                   time = now.ToString("HH:mm:ss"),
                   path = string.Format(@"{0}\{1}.txt", logfile, name),
                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine;
            File.AppendAllText(path, txt);
        }
    }
}
