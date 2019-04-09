using System;
using System.Configuration;

namespace ZapLib.Utility
{
    /// <summary>
    /// 取得應用程式 Config 設定的輔助工具
    /// </summary>
    public static class Config
    {

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
