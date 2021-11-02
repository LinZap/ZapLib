using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ZapLib
{
    /// <summary>
    /// zip 壓縮類別輔助工具
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 將被壓縮的目標目錄名稱
        /// </summary>
        public string ZipDirName { get; private set; }

        /// <summary>
        /// 將被壓縮的目標目錄資訊物件
        /// </summary>
        public DirectoryInfo Folder { get; private set; }

        /// <summary>
        /// 錯誤訊息紀錄
        /// </summary>
        public List<string> ErrorLogs { get; private set; }
        
        /// <summary>
        /// 將被加入壓縮檔中的檔案列表
        /// </summary>
        public List<(string, string, string)> FileList { get; private set; }

        /// <summary>
        /// 最終壓縮檔案的實體路徑位置
        /// </summary>
        public string ZipDist { get; private set; }

        private string ZipFileNamePath;
        private string Root;

        /// <summary>
        /// 建構子，建立壓縮工具初始化必要資訊
        /// </summary>
        /// <param name="foldername">壓縮檔案的目錄名稱，最終檔案將會加上 .zip</param>
        /// <param name="root">存放的位置</param>
        public ZipHelper(string foldername, string root = null)
        {
            FileList = new List<(string, string, string)>();
            ErrorLogs = new List<string>();
            if (string.IsNullOrWhiteSpace(foldername)) ZipDirName = Guid.NewGuid().ToString();
            ZipDirName = foldername.Trim();
            this.Root = root;
            if (string.IsNullOrWhiteSpace(this.Root)) this.Root = ".";
            ZipFileNamePath = Path.Combine(this.Root, ZipDirName);
            while (Directory.Exists(ZipFileNamePath))
            {
                ZipDirName += "-" + Guid.NewGuid().ToString();
                ZipFileNamePath = Path.Combine(this.Root, ZipDirName);
            }
            Folder = Directory.CreateDirectory(ZipFileNamePath);
        }

        /// <summary>
        /// 新增檔案到 zip 壓縮檔中
        /// </summary>
        /// <param name="target_folder_path">放置在壓縮檔案中的目錄的相對路徑 (目錄將自動建立)</param>
        /// <param name="target_file_path">實體檔案放置位置</param>
        /// <param name="newfilename">檔案新名稱，如果沒給則使用原始名稱</param>
        public void AddFile(string target_folder_path, string target_file_path, string newfilename = null)
        {
            // proc file name
            string filename = Path.GetFileName(target_file_path);
            if (!string.IsNullOrWhiteSpace(newfilename)) filename = newfilename;
            if (string.IsNullOrWhiteSpace(filename)) filename = Guid.NewGuid().ToString();

            // proc folder
            string[] subfolder_names = ParseFolder(target_folder_path);
            DirectoryInfo current_folder = Folder;
            foreach (string subfolder_name in subfolder_names)
            {
                try
                {
                    current_folder = current_folder.CreateSubdirectory(subfolder_name);
                    //Trace.WriteLine("[create folder]" + subfolder_name);
                }
                catch (Exception e)
                {
                    ErrorLogs.Add(e.ToString());
                }
            }
            FileList.Add((current_folder.FullName, target_file_path, filename));
        }

        /// <summary>
        /// 壓縮檔案，並清除暫存檔
        /// </summary>
        /// <returns>是否壓縮成功，如果壓縮成功可以從成員 ZipDist 取得 zip 檔案路徑</returns>
        public bool Zip()
        {
            try
            {
                foreach ((string folderpath, string filepath, string filename) in FileList)
                {
                    File.Copy(filepath, folderpath + "\\" + filename);
                }

                ZipDist = ZipFileNamePath + ".zip";

                while (File.Exists(ZipDist)){
                    ZipDist = ZipFileNamePath + "-" + Guid.NewGuid().ToString() + ".zip";
                }

                ZipFile.CreateFromDirectory(ZipFileNamePath, ZipDist, CompressionLevel.Fastest, true);

                if (File.Exists(ZipDist)) return RemoveTmp();
                else
                {
                    ErrorLogs.Add("zip file, and zip file not exists: " + ZipDist);
                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorLogs.Add(e.ToString());
            }
            return false;
        }

        /// <summary>
        /// 刪除暫存檔案，此函數會在 Zip() 方法完成後自動呼叫
        /// </summary>
        /// <returns></returns>
        public bool RemoveTmp()
        {
            try
            {
                if (Directory.Exists(ZipFileNamePath))
                {
                    Directory.Delete(ZipFileNamePath, true);
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogs.Add(e.ToString());

            }
            return false;
        }

        /// <summary>
        /// 解析相對路徑，但排除 . 與 .. 兩種格式的操作符號，回傳各層目錄名稱列表
        /// </summary>
        /// <param name="path">相對路徑字串</param>
        /// <returns>依照目錄階層回傳有序的目錄陣列</returns>
        public string[] ParseFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new string[] { };
            //string new_path = Path.GetDirectoryName(path);
            string[] folders = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            return folders.Where(s => !string.IsNullOrWhiteSpace(s) && !s.Contains(".")).ToArray();
        }


    }
}

