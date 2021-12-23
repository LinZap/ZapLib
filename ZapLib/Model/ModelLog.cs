namespace ZapLib.Model
{
    /// <summary>
    /// 讀取 Log 檔案的回傳資料模型
    /// </summary>
    public class ModelLog
    {
        /// <summary>
        /// log 訊息資料
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 目前頁數
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 最大頁數
        /// </summary>
        public int MaxPage { get; set; }

        /// <summary>
        /// 頁面大小 (byte)
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 取得結果
        /// </summary>
        public bool Result { get; set; }
        
    }
}
