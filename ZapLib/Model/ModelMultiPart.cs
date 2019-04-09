using System.Collections.Generic;
using System.Collections.Specialized;

namespace ZapLib.Model
{
    /// <summary>
    /// 從 Multi-Part Form 中取得對應的模型
    /// </summary>
    public class ModelMultiPart
    {
        /// <summary>
        /// 資料部分
        /// </summary>
        public NameValueCollection body { set; get; }
        /// <summary>
        /// 檔案部分
        /// </summary>
        public List<ModelFile> files { set; get; }
    }
}
