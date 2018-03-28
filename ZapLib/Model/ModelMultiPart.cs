using System.Collections.Generic;
using System.Collections.Specialized;

namespace ZapLib.Model
{
    public class ModelMultiPart
    {
        public NameValueCollection body { set; get; }
        public List<ModelFile> files { set; get; }
    }
}
