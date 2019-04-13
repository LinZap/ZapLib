using ZapLib.Utility;

namespace ZapLib.Json
{
    /// <summary>
    /// 純值 Tuple 用來表示 JSON 中純值資料
    /// </summary>
    public class ValueTuple : IJsonTuple
    {
        private object value;

        /// <summary>
        /// 純值無法再使用索引進行展開，展開後一律返回空的純值 Tuple
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[int idx]
        {
            get => new ValueTuple(null);
            set { }
        }

        /// <summary>
        /// 純值無法再使用索引進行展開，展開後一律返回空的純值 Tuple
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        public IJsonTuple this[string idx]
        {
            get => new ValueTuple(null);
            set { }
        }

        /// <summary>
        /// 建構子，指定純值資料
        /// </summary>
        /// <param name="value">純值</param>
        public ValueTuple(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// 取得目前實值，將使用指定類型轉換，如轉換不過時，將使用預設值回傳
        /// </summary>
        /// <returns>目前轉換後的的數值</returns>
        public T Value<T>(T default_value) => Cast.To(value, default_value);

        /// <summary>
        /// 直接回傳目前實值
        /// </summary>
        /// <returns>目前實值</returns>
        public object Value(object default_value = null) => value;
    }
}
