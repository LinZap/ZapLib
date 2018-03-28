using System;
using System.Configuration;

namespace ZapLib
{
    public class Config
    {
        public static string get(string key)
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

        public static string getConnectionString(string key)
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
