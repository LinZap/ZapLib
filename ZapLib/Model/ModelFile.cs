namespace ZapLib.Model
{
    /// <summary>
    /// 基本檔案資料模型
    /// </summary>
    public class ModelFile
    {
        /// <summary>
        /// 系統用 ID，預設為 0
        /// </summary>
        public int oid { set; get; }
        /// <summary>
        /// 檔案新名稱
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 檔案的 Content-Type
        /// </summary>
        public string type { set; get; }
        /// <summary>
        /// 檔案原始檔名
        /// </summary>
        public string des { set; get; }
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long size { get; set; }

        public ModelFile()
        {
            size = 0;
        }
    }
}
