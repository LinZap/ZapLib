using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;

namespace ZapLib.Utility
{
    /// <summary>
    /// 取得應用程式 Config 設定的輔助工具
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 刷新 .config 設定檔到記憶體中，預設刷新 appSettings 與 DriveConnectionString 兩個區域
        /// </summary>
        /// <param name="section">指定區域，可以不給</param>
        public static void Refresh(string section = null)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            else
            {
                ConfigurationManager.RefreshSection(section);
            }

        }
        /// <summary>
        /// 取得 App.config 或 Web.config 的 appSetting 中指定名稱的數值，取不到時將回傳 NULL
        /// </summary>
        /// <param name="key">指定名稱</param>
        /// <returns>數值</returns>
        public static string Get(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 取得 App.config 或 Web.config 的 appSetting 中的所有設定
        /// </summary>
        /// <returns>所有設定 NameValueCollection</returns>
        public static NameValueCollection Get()
        {
            try
            {
                return ConfigurationManager.AppSettings;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 設定既有的或新增全新的 Config 參數 (已存在則會修改，不存在則會新增)
        /// </summary>
        /// <param name="key">參數名稱</param>
        /// <param name="val">參數數值</param>
        /// <returns>回傳是否執行成功</returns>
        public static bool SetOrAdd(string key, string val)
        {
            Configuration configFile;
            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    configFile = WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                AppSettingsSection section = (AppSettingsSection)configFile.GetSection("appSettings");
                KeyValueConfigurationCollection setting = section.Settings;
                if (setting[key] == null)
                {
                    setting.Add(key, val);
                }
                else
                {
                    setting[key].Value = val;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {
                new MyLog().Write(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 設定既有的或新增全新的資料庫連線字串 (已存在則會修改，不存在則會新增)
        /// </summary>
        /// <param name="key">連線字串名稱</param>
        /// <param name="val">連線字串</param>
        /// <param name="providerName">資料提供者的名稱</param>
        /// <returns></returns>
        public static bool SetOrAddConnectionString(string key, string val, string providerName = "System.Data.SqlClient")
        {
            try
            {
                Configuration configFile;
                if (System.Web.HttpContext.Current != null)
                {
                    configFile = WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                ConnectionStringsSection section = (ConnectionStringsSection)configFile.GetSection("connectionStrings");
                ConnectionStringSettingsCollection setting = section.ConnectionStrings;
                if (setting[key] == null)
                {
                    ConnectionStringSettings item = new ConnectionStringSettings(key, val, providerName);
                    setting.Add(item);
                }
                else
                {
                    setting[key].ConnectionString = val;
                    setting[key].ProviderName = providerName;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch(Exception e)
            {
                new MyLog().Write(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得所有資料庫連線字串設定，如果無法取得則回傳 null
        /// </summary>
        /// <returns>所有資料庫連線字串設定集合</returns>
        public static ConnectionStringSettingsCollection GetConnectionStrings()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得  App.config 或 Web.config 的 ConnectionStrings 中指定名稱的數值，取不到時將回傳 NULL
        /// </summary>
        /// <param name="key">指定名稱</param>
        /// <returns>數值</returns>
        public static string GetConnectionString(string key)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[key].ConnectionString;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
