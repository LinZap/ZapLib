# Utility - Config

`Config` 靜態類別封裝 .NET 的 `App.config` / `Web.config` 讀寫。比起每次都打 `ConfigurationManager.AppSettings[key]`，`Config.Get(key)` 短一點、且取不到時回 `null` 而非 throw。

## Namespace

```csharp
using ZapLib.Utility;
```

## Read appSettings

```xml
<appSettings>
  <add key="ApiTimeout" value="30" />
  <add key="LogPath" value="D:\logs" />
</appSettings>
```

```csharp
string timeout = Config.Get("ApiTimeout");   // "30"
string notExist = Config.Get("XXX");          // null（不 throw）

// 全部
NameValueCollection all = Config.Get();
foreach (string key in all)
{
    Console.WriteLine($"{key} = {all[key]}");
}
```

> **永不 throw**：`Config.Get(key)` 對任何 key 都返回 `string?`，找不到回 `null`。

## Read connectionStrings

```xml
<connectionStrings>
  <add name="DefaultConn" connectionString="Server=...;Database=..." />
</connectionStrings>
```

```csharp
string conn = Config.GetConnectionString("DefaultConn");

// 全部
ConnectionStringSettingsCollection all = Config.GetConnectionStrings();
```

## Write appSettings

`SetOrAdd` 不存在時新增、存在時覆蓋：

```csharp
Config.SetOrAdd("ApiTimeout", "60");
Config.SetOrAdd("NewKey", "hello");
```

**寫入後檔案會立即儲存**，並自動 refresh memory。

`Delete` 不存在時也回傳 `true`（idempotent）：

```csharp
Config.Delete("OldKey");
```

## Write connectionStrings

```csharp
// 新增 / 更新
Config.SetOrAddConnectionString(
    key: "OracleConn",
    val: "User Id=zaplib;Password=...;Data Source=10.0.0.1:1521/PDB",
    providerName: "Oracle.ManagedDataAccess.Client"
);

// 刪除
Config.DeleteConnectionString("OracleConn");
```

> `providerName` 預設 `System.Data.SqlClient`。連 Oracle 必須改為 `Oracle.ManagedDataAccess.Client`，詳見 [SQL / Oracle](../sql/oracle.md)。

## Refresh

外部程式改了 `App.config`，要讓本 process 重讀：

```csharp
Config.Refresh();                   // 刷新 appSettings + connectionStrings
Config.Refresh("appSettings");       // 只刷新 appSettings
```

## Auto-Detect Web vs Desktop

`Config.SetOrAdd` / `Delete` 系列**會自動判斷**目前是 Web 還是 Console / Desktop 程式：

* `HttpContext.Current != null` → 走 `WebConfigurationManager.OpenWebConfiguration("~")`
* 否則 → 走 `ConfigurationManager.OpenExeConfiguration()`

意思是 **同一行程式碼**在 Web API、Worker Service、Console app 都能正確運作。

## Common Patterns

### Read with Default

```csharp
public static int GetTimeoutSeconds()
{
    return int.TryParse(Config.Get("ApiTimeout"), out int v) ? v : 30;
}
```

或借用 `Cast`：

```csharp
using ZapLib.Utility;

int timeout = Cast.To<int>(Config.Get("ApiTimeout"), def_val: 30);
```

### Feature Flag

```csharp
public static bool IsFeatureXEnabled()
{
    return string.Equals(Config.Get("FeatureX"), "true", StringComparison.OrdinalIgnoreCase);
}
```

### Lazy Cached Config

如果 config 讀取在熱路徑上，`ConfigurationManager` 內部已經有 cache，但你仍可在程式層做：

```csharp
public static class Settings
{
    public static readonly Lazy<int> ApiTimeout = new Lazy<int>(
        () => int.TryParse(Config.Get("ApiTimeout"), out int v) ? v : 30
    );
}

// 使用
int timeout = Settings.ApiTimeout.Value;
```

## ZapLib 內部使用的 Config Keys

許多 ZapLib 類別會讀 config 來調整行為：

| Key | 使用者 | 說明 |
|---|---|---|
| `SilentMode` | `MyLog` | `true` 時關閉 log 輸出 |
| `TLS12` | `Fetch` | `true` 時 `Send()` 前強制套用 TLS 1.2 |
| `DBHost` / `DBName` / `DBAct` / `DBPwd` | `SQL()` 無參數建構子 | SQL Server 連線資訊 |
| `EnableDBAlwaysOn` | `SQL` | 啟用 AlwaysOn 唯讀路由 |
| `SQLDBReplace` + `SQLDBReplace:xxx` | `SQL` | 跨環境的 DB name 動態替換 |
| `Storage` | `ExtApiHelper.UploadFile()` | 檔案上傳目錄 |
| `MaxUploadFileSize` | `ExtApiHelper.UploadFile()` | 最大上傳大小（bytes） |
| `APIDataLimit` | `ExtApiHelper.AddPaging` | API 翻頁的 max page size |

詳見 [Global Config](../global-config.md)。
