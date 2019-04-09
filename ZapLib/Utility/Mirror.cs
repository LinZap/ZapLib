using System;
using System.Collections;
using System.Collections.Generic;

namespace ZapLib.Utility
{
    /// <summary>
    /// 萬用的反射輔助工具
    /// </summary>
    public static class Mirror
    {
        /// <summary>
        /// 反射指定物件的成員，並以迭代方式回傳成員的名稱與數值
        /// </summary>
        /// <param name="obj">要反射的物件</param>
        /// <returns>迭代器，反覆返回 DictionaryEntry，該物件的 Key 與 Value 即為成員的名稱與數值</returns>
        public static IEnumerable<DictionaryEntry> Members(object obj)
        {
            if (obj == null) yield break;
            if (Cast.IsType<IDictionary>(obj))
                foreach (DictionaryEntry member in (IDictionary)obj)
                    yield return member;
            else
                foreach (var prop in obj.GetType().GetProperties())
                    yield return new DictionaryEntry(prop.Name, prop.GetValue(obj, null));
        }

        /// <summary>
        /// 反射指定物件的成員，並將成員名稱轉換為 TKey 型態、成員數值轉換成 TVal 型態，傳遞給回呼 (callback) 委派函數處理，當轉換不過時將回傳該型態的預設值
        /// </summary>
        /// <typeparam name="TKey">物件成員名稱轉換成這個型態</typeparam>
        /// <typeparam name="TVal">物件成員數值轉換成這個型態</typeparam>
        /// <param name="obj">要反射的物件</param>
        /// <param name="cb">回呼 (callback) 委派函數</param>
        public static void EachMembers<TKey, TVal>(object obj, Action<TKey, TVal> cb)
        {
            if (cb == null) return;
            foreach (var kv in Members(obj))
                cb(Cast.To<TKey>(kv.Key), Cast.To<TVal>(kv.Value));
        }

        /// <summary>
        /// 物件成員數值合併，將數個物件 objs 由前到後依序將其成員數值覆蓋到目標物件 target 中，成員名稱與型態必須相同才會覆蓋，否則將忽略
        /// </summary>
        /// <typeparam name="T">目標物件類型 T</typeparam>
        /// <param name="target">目標物件</param>
        /// <param name="objs">數個任意物件</param>
        public static void Assign<T>(ref T target, params object[] objs)
        {
            foreach (object obj in objs)
                foreach (var kv in Members(obj))
                    AssignValue(ref target, kv.Key, kv.Value);
        }

        /// <summary>
        /// 將目標物件的特定成員 key 覆蓋數值 value，如果找不到該成員或該成員唯讀，將不進行任何操作
        /// </summary>
        /// <typeparam name="T">目標物件類型 T</typeparam>
        /// <param name="target">目標物件</param>
        /// <param name="key">指定成員名稱</param>
        /// <param name="value">指定成員數值</param>
        /// <returns></returns>
        public static bool AssignValue<T>(ref T target, object key, object value)
        {
            try
            {
                if (Cast.IsType<IDictionary>(target))
                {
                    IDictionary dit = (IDictionary)target;
                    if (dit.Contains(key))
                    {
                        value = Cast.To(value, dit.GetType().GetGenericArguments()[1]);
                        if (value != null) dit[key] = value;
                    }
                }
                else
                {
                    var prop = target.GetType().GetProperty(Convert.ToString(key));
                    if (prop != null && prop.CanWrite)
                    {
                        value = Cast.To(value, prop.PropertyType);
                        if (value != null) prop.SetValue(target, value);
                    }
                }
            }
            catch 
            {
                return false;
            }
            return true;
        }

    }
}
