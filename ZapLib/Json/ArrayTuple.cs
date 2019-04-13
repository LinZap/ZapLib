using System.Collections.Generic;
using ZapLib.Utility;

namespace ZapLib.Json
{
    /// <summary>
    /// 陣列 Tuple 用來表示 JSON 中陣列資料
    /// </summary>
    public class ArrayTuple : IJsonTuple
    {
        private List<IJsonTuple> data;

        /// <summary>
        /// 使用數字做為索引子進行展開
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[int idx]
        {
            get => _get(idx);
            set => _set(idx, value);
        }

        /// <summary>
        /// 使用字串做為索引子進行展開，字串將被嘗試轉換成字串，如果轉換不過時，將使用 -1
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[string idx]
        {
            get => _get(Cast.To(idx, -1));
            set => _set(Cast.To(idx, -1), value);
        }

        private void _set(int idx, IJsonTuple value)
        {
            if (data == null || idx >= data.Count || idx < 0) return;
            data[idx] = value;
        }

        private IJsonTuple _get(int idx) => data == null || idx >= data.Count || idx < 0 ? new ValueTuple(null) : data[idx];

        /// <summary>
        /// 建構子，建立陣列資料結構
        /// </summary>
        /// <param name="create_init_object">是否建立初始陣列</param>
        public ArrayTuple(bool create_init_object = true)
        {
            if (create_init_object) data = new List<IJsonTuple>();
        }

        /// <summary>
        /// 取得實值，資料為內部的 List 陣列資料 
        /// </summary>
        /// <returns>List 陣列資料</returns>
        public List<IJsonTuple> Value()
        {
            return data;
        }

        /// <summary>
        /// 取得目前實值，將使用指定類型轉換，如轉換不過時，將使用預設值回傳
        /// </summary>
        /// <returns>目前轉換後的的數值</returns>
        public T Value<T>(T default_value) => Cast.To(data, default_value);

        /// <summary>
        /// 直接回傳目前實值
        /// </summary>
        /// <returns>目前實值</returns>
        public object Value(object default_value) => data ?? default_value;
    }
}
