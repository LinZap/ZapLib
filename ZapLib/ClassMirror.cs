using System;
using System.Reflection;

namespace ZapLib
{
    /// <summary>
    /// Type 鏡像類別產生工具，使用 Type 來產生對應的實體物件
    /// </summary>
    public class ClassMirror
    {
        /// <summary>
        /// 類型
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// 實體物件
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; private set; }

        /// <summary>
        /// 建構子，產生實體物件，無法建立時 Instance 成員會為 null
        /// </summary>
        /// <param name="type">類型</param>
        /// <param name="args">參數(多個)</param>
        public ClassMirror(Type type, params object[] args)
        {
            Type = type;
            try
            {
                Instance = Activator.CreateInstance(Type, args: args);
            }
            catch (Exception e)
            {
                ErrMsg = e.ToString();
            }
        }

        /// <summary>
        /// 在指定的類型中取得指定名稱的方法，無法取得時回傳 null
        /// </summary>
        /// <param name="MethodName">指定方法名稱</param>
        /// <returns>方法的資訊</returns>
        public MethodInfo this[string MethodName] => GetMethod(MethodName);


        private MethodInfo GetMethod(string MethodName)
        {
            try
            {
                if (Instance == null) return null;
                return Type.GetMethod(MethodName);

            }
            catch (Exception e)
            {
                ErrMsg = e.ToString();
            }
            return null;
        }

    }
}
