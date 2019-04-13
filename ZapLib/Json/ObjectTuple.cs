using System.Collections.Generic;
using ZapLib.Utility;

namespace ZapLib.Json
{
    /// <summary>
    /// 物件 Tuple 用來表示 JSON 中物件資料
    /// </summary>
    public class ObjectTuple : IJsonTuple
    {
        private Dictionary<string, IJsonTuple> data;

        /// <summary>
        /// 使用數字做為索引子進行展開，會嘗試將數字轉換成字串，轉換不過時將使用 "0"
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[int idx]
        {
            get => _get(Cast.To(idx, "0"));
            set => _set(Cast.To(idx, "0"), value);
        }

        /// <summary>
        /// 使用字串做為索引子進行展開
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[string idx]
        {
            get => _get(idx);
            set => _set(idx, value);
        }

        private void _set(string key, IJsonTuple val)
        {
            if (data == null || key == null) return;
            data[key] = val;
        }

        private IJsonTuple _get(string key) => data == null || !data.ContainsKey(key) ? new ValueTuple(null) : data[key];

        /// <summary>
        /// 建構子，建立字典物件資料結構
        /// </summary>
        /// <param name="create_init_object">是否建立初始字典物件</param>
        public ObjectTuple(bool create_init_object = true)
        {
            if (create_init_object) data = new Dictionary<string, IJsonTuple>();
        }

        /// <summary>
        /// 取得實值，資料為內部的 Dictionary 物件資料 
        /// </summary>
        /// <returns>Dictionary 字典物件</returns>
        public Dictionary<string, IJsonTuple> Value()
        {
            return data;
        }

        /// <summary>
        /// 取得目前實值，將使用指定類型轉換，如轉換不過時，將使用 default 回傳
        /// </summary>
        /// <returns>目前轉換後的的數值</returns>
        public T Value<T>(T default_value = default) => Cast.To(data, default_value);

        /// <summary>
        /// 直接回傳目前實值
        /// </summary>
        /// <returns>目前實值</returns>
        public object Value(object default_value = null) => data ?? default_value;
    }
}
