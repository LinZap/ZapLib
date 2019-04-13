namespace ZapLib.Json
{
    /// <summary>
    /// Json 資料存取的基本元素介面，它將允許使用索引方式存取資料
    /// </summary>
    public interface IJsonTuple
    {
        /// <summary>
        /// 使用數字做為索引子進行展開
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        IJsonTuple this[int idx] { get; set; }

        /// <summary>
        /// 使用字串做為索引子進行展開
        /// </summary>
        /// <param name="idx">索引子</param>
        /// <returns>展開後仍然回傳 IJSONTuple</returns>
        IJsonTuple this[string idx] { get; set; }

        /// <summary>
        /// 取得目前實值，將使用指定類型轉換，如轉換不過時，將使用 default 回傳
        /// </summary>
        /// <returns>目前轉換後的的數值</returns>
        T Value<T>(T default_value = default);

        /// <summary>
        /// 直接回傳目前實值
        /// </summary>
        /// <returns>目前實值</returns>
        object Value(object default_value = null);
    }
}
