namespace ZapLib.Model
{
    public class ModelFile
    {
        public int oid { set; get; }
        public string name { set; get; }
        public string type { set; get; }
        public string des { set; get; }
        public long size { get; set; }
        public ModelFile()
        {
            size = 0;
        }
    }
}
