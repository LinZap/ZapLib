using System;
using System.IO;

namespace ZapLib
{
    public class MyLog
    {
        public string path { get; set; }
        public string name { get; set; }

        public string silentMode { get; set; }

        public void write(string msg)
        {
            if (silentMode != null)
            {
                bool _silentMode = false;
                bool.TryParse(silentMode, out _silentMode);
                if (_silentMode) return;
            }
             
            string logfile = path ?? Config.get("Storage");
            if (logfile == null) return;

            DateTime now = DateTime.Now;
            string name = this.name ?? now.ToString("yyyyMMdd"),
                   time = now.ToString("HH:mm:ss"),
                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine,
                   lastpath = string.Format(@"{0}\{1}", logfile, name);
            
            _write(lastpath, txt);
        }

        private void _write(string path,string content)
        {
            try
            {
                File.AppendAllText(path + ".txt", content);
            }
            catch(Exception e)
            {
                if (Config.get("ForceLog") != null)
                {
                    path += "-"+Guid.NewGuid().ToString();
                    content = e.ToString() + "\n" + content;
                    _write(path, content);
                }      
            }
        }
    }
}
