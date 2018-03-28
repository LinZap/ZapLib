using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZapLib
{
    public class QueryString
    {
        /*
            parse object to query string
            @object: the any object
            if object is null then return null 
            else format object to querystring, 
            like this: "key=val&key=val&..."
        */
        public static string Parse(object data)
        {
            string result = null;
            if (data == null) return "";

            List<string> para = new List<string>();

            if (data.GetType() == typeof(Dictionary<string, string>))
            {
                foreach (KeyValuePair<string, string> prop in (Dictionary<string, string>)data)
                    para.Add(string.Format("{0}={1}", prop.Key, prop.Value));
            }
            else
            {
                try
                {
                    foreach (var prop in data.GetType().GetProperties())
                    {
                        object value = prop.GetValue(data, null);
                        if (value != null)
                            para.Add(string.Format("{0}={1}", prop.Name, value.ToString()));
                    }
                }
                catch (Exception)
                {
                    MyLog log = new MyLog();
                    log.write("can not parse object to query string: " + JsonConvert.SerializeObject(data));
                }
            }

            result = string.Join("&", para.ToArray());
            return result;
        }

        /*
            leftpad query string into object
        */
        public static T Objectify<T>(string qs)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            if (qs != null)
            {
                string[] sp = qs.Split('&');
                foreach (string pair in sp)
                {
                    string[] kv = pair.Split('=');
                    if (kv.Length > 2)
                    {
                        string v = "";
                        for (int i = 1; i < kv.Length; i++) v += kv[i];
                        AssignObject(ref obj, kv[0], v);
                    }
                    else if (kv.Length == 2)
                    {
                        AssignObject(ref obj, kv[0], kv[1]);
                    }
                }
            }
            return obj;
        }

        private static void AssignObject<T>(ref T obj, string key, string value)
        {
            var prop = obj.GetType().GetProperty(key);

            if (prop != null && prop.CanWrite)
            {
                if (prop.PropertyType == typeof(int))
                    prop.SetValue(obj, int.Parse(value), null);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(obj, bool.Parse(value), null);
                else
                    prop.SetValue(obj, value, null);

            }
        }
    }
}
