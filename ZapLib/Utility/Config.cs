﻿using System;
using System.Collections.Specialized;
using System.Configuration;

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
                ConfigurationManager.RefreshSection("DriveConnectionString");
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
