# Web API - ExtApiHelper

`ExtApiHelper` 是 ZapLib 對 ASP.NET Web API 2 `ApiController` 的擴充輔助工具。它把「拿 Header / 拿 IP / 設 Cookie / 回 JSON / 回檔案下載 / 收檔案上傳」這些日常瑣事壓成一兩行 API。

## Namespace

```csharp
using ZapLib;
using ZapLib.Model;
```

## Setup

在 `ApiController` 內建立一個 `ExtApiHelper`，傳入 `this`：

```csharp
public class OrderController : ApiController
{
    private ExtApiHelper api;

    public OrderController()
    {
        // 注意：建構時 Request 還是 null，這裡只是先建立
    }

    [HttpGet]
    public HttpResponseMessage GetOrder(int id)
    {
        api = new ExtApiHelper(this);
        // ... 業務邏輯
        return api.GetResponse(new { id, status = "ok" });
    }
}
```

或者更乾脆，每個 action 開頭建一個：

```csharp
[HttpGet]
public HttpResponseMessage GetOrder(int id)
{
    var api = new ExtApiHelper(this);
    return api.GetResponse(new { id, status = "ok" });
}
```

## Read from Request

### Headers

```csharp
// 所有 Headers
Dictionary<string, IEnumerable<string>> all = api.GetHeaders();

// 單一 Header
string token = api.GetHeader("Authorization");

// 帶預設值
string lang = api.GetHeader("Accept-Language", "zh-TW");

// 自動轉型
int retryCount = api.GetHeader<int>("X-Retry-Count", def_val: 0);
```

### IP & User Agent

```csharp
string clientIp = api.GetIP();           // 用戶端 IP
string ua = api.GetUserAgent();          // 瀏覽器 UA
string myHost = api.GetMyHost();         // 我自己的 URL: https://api.example.com:443
```

### Query String

```csharp
// URL: /api/orders?page=2&limit=20
string page = api.GetQuery("page");      // "2"
string limit = api.GetQuery("limit");    // "20"
```

### Cookies

```csharp
string sessionId = api.GetCookie("sessionid");
```

### Body — JSON

```csharp
public class CreateOrderRequest
{
    public int userId { get; set; }
    public decimal amount { get; set; }
}

[HttpPost]
public HttpResponseMessage Create()
{
    var api = new ExtApiHelper(this);
    CreateOrderRequest req = api.GetJsonBody<CreateOrderRequest>();

    // ... 處理
    return api.GetResponse(new { ok = true });
}
```

### Body — Form

```csharp
[HttpPost]
public HttpResponseMessage Submit()
{
    var api = new ExtApiHelper(this);
    var form = api.GetFormBody<CreateOrderRequest>();
    return api.GetResponse(new { ok = true });
}
```

### Body — File Upload

`UploadFile()` 會把上傳的檔案存到 `Config["Storage"]` 設定的路徑下，並回傳檔案資訊：

```xml
<appSettings>
  <add key="Storage" value="D:\uploads" />
  <add key="MaxUploadFileSize" value="10485760" /> <!-- 10 MB -->
</appSettings>
```

```csharp
[HttpPost]
public HttpResponseMessage Upload()
{
    var api = new ExtApiHelper(this);
    List<ModelFile> files = api.UploadFile();

    if (files == null) return api.GetResponse(new { error = "no file" }, HttpStatusCode.BadRequest);

    return api.GetResponse(files);
}
```

回傳的 `ModelFile`：

| 屬性 | 內容 |
|---|---|
| `name` | 儲存後的新檔名（`{GUID}_{原檔名}`） |
| `des` | 原始檔名 |
| `size` | 檔案大小（bytes） |
| `type` | MIME type |

> 超過 `MaxUploadFileSize`（預設 5 MB）的檔案會被自動丟棄。

## Write to Response

### JSON Response

```csharp
return api.GetResponse(new
{
    code = 200,
    data = new { id = 1, name = "ZapLib" }
});
```

帶 HTTP 狀態碼：

```csharp
return api.GetResponse(
    new { error = "not found" },
    HttpStatusCode.NotFound
);
```

### Text / HTML Response

```csharp
return api.GetTextResponse("<h1>Hello</h1>", HttpStatusCode.OK);
```

### Redirect

```csharp
[HttpGet]
public HttpResponseMessage Login()
{
    var api = new ExtApiHelper(this);
    // 1 秒後跳轉，預設顯示 "跳轉中，請稍候..."
    return api.GetRedirectResponse("https://example.com/dashboard");
}
```

自訂秒數與顯示文字：

```csharp
return api.GetRedirectResponse("https://example.com", second: 3, wording: "處理中...");
```

### File Download

從**檔案路徑**回應：

```csharp
[HttpGet]
public HttpResponseMessage DownloadReport(int id)
{
    var api = new ExtApiHelper(this);
    return api.GetStreamResponse(
        file: $@"D:\reports\{id}.pdf",
        name: $"report-{id}.pdf",
        type: "application/pdf"
    );
}
```

從 **`byte[]`**：

```csharp
byte[] data = GenerateExcel();
return api.GetStreamResponse(data, name: "export.xlsx",
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
```

從 **`Stream`**：

```csharp
Stream s = OpenSomeStream();
return api.GetStreamResponse(s, name: "data.bin");
```

### Force Inline (不下載、瀏覽器直接開)

```csharp
return api.GetStreamResponse(
    file: @"D:\images\preview.jpg",
    type: "image/jpeg",
    disposition: "inline"
);
```

### Attachment from Text

直接把字串內容塞成下載檔案：

```csharp
string csv = "id,name\n1,ZapLib\n2,Demo";
return api.GetAttachmentResponse(csv, filename: "export.csv");
```

## Set Response Headers / Cookies

```csharp
api.SetHeader("X-Trace-Id", Guid.NewGuid().ToString());

api.AddCookie("token", "abc123", expired: DateTime.UtcNow.AddDays(7));

return api.GetResponse(new { ok = true });
```

> Cookie 預設是 `HttpOnly` + path `/`，domain 設為 request 的 host（`localhost` 例外，會設成 `null`）。

## Pagination

### Identity-Based Paging（推薦）

用某個唯一識別欄位做「下一頁」基準，效能比 `OFFSET` 好：

```csharp
[HttpGet]
public HttpResponseMessage List()
{
    var api = new ExtApiHelper(this);
    var db = new SQL("DefaultConn");

    string sql = "SELECT id, title, since FROM Articles";
    string nextId = api.GetQuery("nextId");

    api.AddIdentityPaging(ref sql, orderby: "since desc", idcolumn: "id", nextId: nextId);

    var rows = db.QuickQuery<Article>(sql);
    return api.GetResponse(rows);
}
```

> 用戶端 URL：`/api/articles?limit=20&nextId=abc-123-...`

### Legacy AddPaging（已棄用）

```csharp
[Obsolete("這個方法可能在下個版本中棄用")]
public void AddPaging(ref string sql, string orderby = "asc")
```

新程式碼請用 `AddIdentityPaging`。

## See Also

* [SignalR](signalr.md) — 在 ApiController 中操作 SignalR Hub
* [ValidPlatform](valid-platform.md) — 平台級驗證
