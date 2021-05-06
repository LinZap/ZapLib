using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ZapLib.Utility;

namespace ZapLib
{
    /// <summary>
    /// 紀錄事件與日誌工具
    /// </summary>
    public class MyLog
    {
        /// <summary>
        /// 日誌存放路徑
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 日誌名稱，預設以 yyyy-mm-dd 進行命名，每天只會產生一個檔案
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 使否啟用安靜模式，預設為 NULL，啟用後本類別將不再寫檔
        /// </summary>
        public string SilentMode { get; set; }

        /// <summary>
        /// 將訊息寫入實體日誌檔案
        /// </summary>
        /// <param name="msg">訊息</param>
        public void Write(string msg)
        {
            if (SilentMode != null)
            {
                bool.TryParse(SilentMode, out bool _silentMode);
                if (_silentMode) return;
            }

            string logfile = Path ?? Config.Get("Storage");

            if (logfile == null) return;
            if (!Directory.Exists(logfile)) return;

            DateTime now = DateTime.Now;
            string name = this.Name ?? now.ToString("yyyyMMdd"),
                   time = now.ToString("HH:mm:ss"),
                   txt = string.Format("[{0}] {1}", time, msg) + Environment.NewLine,
                   lastpath = string.Format(@"{0}\{1}", logfile, name);

            _write(lastpath, txt);
        }

        private void _write(string path, string content)
        {
            try
            {
                File.AppendAllText(path + ".txt", content);
            }
            catch (Exception e)
            {
                if (Config.Get("ForceLog") != null)
                {
                    path += "-" + Guid.NewGuid().ToString();
                    content = e.ToString() + "\n" + content;
                    _write(path, content);
                }
            }
        }

        /// <summary>
        /// 將例外物件記錄到 Windows 事件檢視器中
        /// </summary>
        /// <param name="ex">例外物件</param>
        /// <param name="type">事件類型，預設為 Error</param>
        /// <param name="ProjectName">請勿設定</param>
        /// <param name="CodePath">請勿設定</param>
        /// <param name="LineNumber">請勿設定</param>
        public void Event(Exception ex, EventLogEntryType type = EventLogEntryType.Error,
            [System.Runtime.CompilerServices.CallerMemberName] string ProjectName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string CodePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int LineNumber = 0)
        {
            _Event(ex.ToString(), type, ProjectName, CodePath, LineNumber);
        }

        /// <summary>
        /// 將指定訊息記錄到 Windows 事件檢視器中
        /// </summary>
        /// <param name="msg">指定訊息</param>
        /// <param name="type">事件類型，預設為 Information</param>
        /// <param name="ProjectName">請勿設定</param>
        /// <param name="CodePath">請勿設定</param>
        /// <param name="LineNumber">請勿設定</param>
        public void Event(string msg, EventLogEntryType type = EventLogEntryType.Information,
                    [System.Runtime.CompilerServices.CallerMemberName] string ProjectName = "",
                    [System.Runtime.CompilerServices.CallerFilePath] string CodePath = "",
                    [System.Runtime.CompilerServices.CallerLineNumber] int LineNumber = 0)
        {
            _Event(msg, type, ProjectName, CodePath, LineNumber);
        }

        private void _Event(string Message, EventLogEntryType type = EventLogEntryType.Error, string ProjectName = "", string CodePath = "", int LineNumber = 0)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    string AppName = AppDomain.CurrentDomain.FriendlyName;
                    string CallingAssembly = Assembly.GetCallingAssembly().GetName().Name;
                    string CallingAssemblyPath = Assembly.GetCallingAssembly().Location;
                    string ExecAssembly = Assembly.GetExecutingAssembly().FullName;
                    string ExecAssemblyPath = Assembly.GetExecutingAssembly().Location;
                    string[] data = new string[]
                    {
                    $"Process = {AppName}" ,
                    $"CallingAssembly = {CallingAssembly}",
                    $"CallingAssemblyPath = {CallingAssemblyPath}" ,
                    $"ExecAssembly = {ExecAssembly}",
                    $"ExecAssemblyPath = {ExecAssemblyPath}" ,
                    $"CodePath = {CodePath}",
                    $"Method = {ProjectName}",
                    $"Log At Line = {LineNumber}",
                    $"Message = {Message}"
                    };
                    eventLog.WriteEvent(new EventInstance(0, 0, type), data);
                }
            }
            catch { }
        }
    }
}
