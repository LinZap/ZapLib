# Utility - ZipHelper

`ZipHelper` 把「在記憶體中拼出檔案結構 → 壓縮成 zip」這個流程包成 3 步驟：**`new ZipHelper` → `AddFile` → `Zip`**。底層用 .NET 內建的 `System.IO.Compression.ZipFile`。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

```csharp
ZipHelper zip = new ZipHelper(
    foldername: "backup-20260520",
    root: @"D:\tmp"
);

// 加檔案 — 第一個參數是 zip 中的相對路徑
zip.AddFile("", @"C:\data\config.xml");
zip.AddFile("reports", @"C:\data\2026-q1.pdf");
zip.AddFile("reports", @"C:\data\2026-q2.pdf");
zip.AddFile("images/icons", @"C:\data\logo.png");

if (zip.Zip())
{
    Console.WriteLine("壓縮成功：" + zip.ZipDist);
}
else
{
    foreach (var err in zip.ErrorLogs) Console.WriteLine(err);
}
```

**輸出**：`D:\tmp\backup-20260520.zip`

**內部結構**：

```
backup-20260520.zip
├── config.xml
├── reports/
│   ├── 2026-q1.pdf
│   └── 2026-q2.pdf
└── images/
    └── icons/
        └── logo.png
```

## How It Works

1. 建構子在 `root` 下**建立一個暫存資料夾**（名稱 = `foldername`）
2. 每次 `AddFile` 把檔案路徑記下來，必要時建立子目錄
3. `Zip()` 把暫存資料夾**整包壓縮**成 `.zip`，再**刪除暫存資料夾**
4. 最終 zip 路徑可從 `ZipDist` 取得

> **重要副作用**：建構子會**立即建立資料夾**。如果你 `new ZipHelper(...)` 後沒呼叫 `Zip()`，暫存資料夾會留在硬碟上 — 記得 `RemoveTmp()` 手動清除。

## Rename File in Zip

第三個參數可改檔名：

```csharp
zip.AddFile("docs", @"C:\data\readme.txt", newfilename: "README.md");
// zip 內：docs/README.md
```

## Path Sanitization

`ZipHelper` 會**過濾 `.` 與 `..`**，避免目錄穿越攻擊：

```csharp
zip.AddFile("../../escape", @"C:\sensitive.txt");
// 實際只用到 "escape"，無法穿越到 ZipDirName 之外
```

## Collision Handling

* **暫存資料夾名稱衝突** → 自動加上 GUID 後綴
* **最終 zip 檔名衝突** → 自動加上 GUID 後綴

```csharp
ZipHelper zip = new ZipHelper("backup", @"D:\tmp");
// 如果 D:\tmp\backup 已存在 → 實際資料夾叫 "backup-{guid}"
// 如果 D:\tmp\backup.zip 已存在 → 實際 zip 叫 "backup-{guid}.zip"
```

## Compression Level

固定使用 `CompressionLevel.Fastest` — 速度優先、壓縮比稍低。

如果需要更高壓縮比，請直接用 `System.IO.Compression.ZipFile`：

```csharp
ZipFile.CreateFromDirectory(srcDir, destZip, CompressionLevel.Optimal, true);
```

## Error Handling

`Zip()` 失敗時回 `false`，所有錯誤累積在 `ErrorLogs`：

```csharp
if (!zip.Zip())
{
    foreach (var err in zip.ErrorLogs)
    {
        Console.WriteLine(err);
    }
}
```

常見原因：

* `AddFile` 給的來源檔案不存在
* 目的地磁碟沒空間
* 沒有目的地的寫入權限
* `root` 路徑不存在

## Real-World Pattern: Web API 動態打包下載

```csharp
[HttpPost]
public HttpResponseMessage DownloadReports([FromBody] int[] reportIds)
{
    var api = new ExtApiHelper(this);
    var db = new SQL("DefaultConn");

    var zip = new ZipHelper($"reports-{DateTime.Now:yyyyMMddHHmmss}", Path.GetTempPath());

    foreach (var id in reportIds)
    {
        ModelReport r = db.QuickQuery<ModelReport>(
            "SELECT * FROM Reports WHERE id = @id",
            new { id },
            isfetchall: false
        )[0];
        zip.AddFile("", r.FilePath);
    }

    if (!zip.Zip())
    {
        return api.GetResponse(new { error = string.Join("\n", zip.ErrorLogs) },
                               HttpStatusCode.InternalServerError);
    }

    return api.GetStreamResponse(zip.ZipDist,
                                  name: Path.GetFileName(zip.ZipDist),
                                  type: "application/zip");
}
```

> 記得另寫一個 cleanup job 定期刪 temp 目錄下的 zip。

## See Also

* [System.IO.Compression.ZipFile (Microsoft Docs)](https://learn.microsoft.com/dotnet/api/system.io.compression.zipfile)
