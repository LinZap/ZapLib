[1mdiff --git a/ZapLib/MyLog.cs b/ZapLib/MyLog.cs[m
[1mindex fe97a80..7fba123 100644[m
[1m--- a/ZapLib/MyLog.cs[m
[1m+++ b/ZapLib/MyLog.cs[m
[36m@@ -7,16 +7,45 @@[m [mnamespace ZapLib[m
     {[m
         public string path { get; set; }[m
         public string name { get; set; }[m
[32m+[m
[32m+[m[32m        public string silentMode { get; set; }[m
[32m+[m
         public void write(string msg)[m
         {[m
[32m+[m[32m            if (silentMode != null)[m
[32m+[m[32m            {[m
[32m+[m[32m                bool _silentMode = false;[m
[32m+[m[32m                bool.TryParse(silentMode, out _silentMode);[m
[32m+[m[32m                if (_silentMode) return;[m
[32m+[m[32m            }[m
[32m+[m[41m             [m
             string logfile = path ?? Config.get("Storage");[m
             if (logfile == null) return;[m
 [m
             DateTime now = DateTime.Now;[m
             string name = this.name ?? now.ToString("yyyyMMdd"),[m
                    time = now.ToString("HH:mm:ss"),[m
[31m-                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine;[m
[31m-            File.AppendAllText(string.Format(@"{0}\{1}.txt", logfile, name), txt);[m
[32m+[m[32m                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine,[m
[32m+[m[32m                   lastpath = string.Format(@"{0}\{1}.txt", logfile, name);[m
[32m+[m[41m            [m
[32m+[m[32m            _write(lastpath, txt);[m
[32m+[m[32m        }[m
[32m+[m
[32m+[m[32m        private void _write(string path,string content)[m
[32m+[m[32m        {[m
[32m+[m[32m            try[m
[32m+[m[32m            {[m
[32m+[m[32m                File.AppendAllText(path + ".txt", content);[m
[32m+[m[32m            }[m
[32m+[m[32m            catch(Exception e)[m
[32m+[m[32m            {[m
[32m+[m[32m                if (Config.get("ForceLog") != null)[m
[32m+[m[32m                {[m
[32m+[m[32m                    path += "-"+Guid.NewGuid().ToString();[m
[32m+[m[32m                    content = e.ToString() + "\n" + content;[m
[32m+[m[32m                    _write(path, content);[m
[32m+[m[32m                }[m[41m      [m
[32m+[m[32m            }[m
         }[m
     }[m
 }[m
