# Fetch - Advanced

進階用法 — Header、Cookie、Proxy、平台驗證、自行操作底層 `HttpClient`。

## Namespace

```csharp
using ZapLib;
```

## Custom Headers

用匿名物件批次設定 Header：

```csharp
Fetch f = new Fetch("https://api.example.com/data");

f.Header = new
{
    Authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5..." ,
    Accept_Language = "zh-TW",
    X_Custom_Token = "abc123"
};

string result = f.Get();
```

> Header 名稱如果含 `-`，C# 屬性名稱無法直接寫，用 `_` 取代。`Fetch` 不會自動轉換 — 如果 server 嚴格要求 `Accept-Language`，請用 `f.Client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW")`。

設定 `Content-Type` 有捷徑：

```csharp
f.ContentType = "application/xml";
```

設定 `Accept`：

```csharp
f.Accept = "application/json";
```

## Cookies

ZapLib 內部用一個 `CookieContainer` 管理 cookie：

```csharp
Fetch f = new Fetch("https://example.com/login");
f.Cookie = new
{
    sessionid = "abc123",
    csrftoken = "xyz789"
};

string result = f.Post(new { username = "admin", password = "secret" });

// 讀回應種下的新 cookie
var cookies = f.Cookie;   // CookieCollection
```

## Proxy

```csharp
Fetch f = new Fetch("https://api.example.com/data");
f.Proxy = "http://proxy.company.com:8080";

string result = f.Get();
```

設定為 `null` 可關閉 proxy（預設值）。

## Request Encoding

預設用 UTF-8 編碼請求 body。如果伺服器要求其他編碼：

```csharp
Fetch f = new Fetch("https://api.example.com/data");
f.RequestEncoding = Encoding.GetEncoding("big5");
```

## Platform Validation (ValidPlatform)

當你需要呼叫**自家內部 API** 並且該 API 用了 `[ValidPlatform]` 屬性保護時，把 `ValidPlatform` 設成 `true`，`Fetch` 會自動產生 `Channel-Signature` / `Channel-Authorization` / `Channel-Iv` 三個 Header：

```csharp
Fetch f = new Fetch("https://internal-api.company.com/admin/data");
f.ValidPlatform = true;

dynamic result = f.Post<dynamic>(new { command = "purge-cache" });
```

> 這個機制需要呼叫端與伺服器端共用同一支金鑰（`ZapLib.Security.Const.Key`）。詳見 [`[ValidPlatform]` Attribute](../webapi/valid-platform.md)。

## Read Response Headers

```csharp
Fetch f = new Fetch("https://api.example.com/data");
f.Get();

// 取單一 header
string contentType = f.GetResponseHeader("Content-Type");
DateTime lastModified = f.GetResponseHeader<DateTime>("Last-Modified");

// 取全部 header
var headers = f.GetResponseHeaders();
foreach (var h in headers)
{
    Console.WriteLine($"{h.Key}: {string.Join(",", h.Value)}");
}
```

`GetResponseHeader<T>` 會自動轉型，失敗時回 `default(T)`：

```csharp
int contentLength = f.GetResponseHeader<int>("Content-Length", defaultVal: 0);
```

## Get Binary Response

POST 之後伺服器可能直接回傳檔案（PDF、圖片）：

```csharp
Fetch f = new Fetch("https://api.example.com/export");
f.Post(new { format = "pdf" });

byte[] data = f.GetBinaryResponse();
File.WriteAllBytes("report.pdf", data);
```

## Direct HttpClient Access

需要的功能 `Fetch` 沒包到？直接操作底層物件：

```csharp
Fetch f = new Fetch("https://api.example.com/data");

// 加 default headers
f.Client.DefaultRequestHeaders.Add("X-Trace-Id", Guid.NewGuid().ToString());

// 設 timeout
f.Client.Timeout = TimeSpan.FromSeconds(60);

// 用 ClientHandler 設定憑證驗證
f.ClientHandler.ServerCertificateCustomValidationCallback =
    (msg, cert, chain, errors) => true;   // 信任全部憑證 (僅測試用!)

string result = f.Get();
```

可存取的底層物件：

| 屬性 | 型別 | 說明 |
|---|---|---|
| `Client` | `HttpClient` | 主要客戶端物件 |
| `ClientHandler` | `HttpClientHandler` | 連線層設定（憑證、Cookie、Proxy） |
| `Request` | `HttpRequestMessage` | 即將送出的請求 |
| `Response` | `HttpResponseMessage` | 回應物件（送出後才有值） |

## Manual Send

不想用 `Get()` / `Post()` 包裝？自行組好 `Request` 再呼叫 `Send()`：

```csharp
Fetch f = new Fetch("https://api.example.com/data");
f.Request.Method = HttpMethod.Get;
f.Request.Headers.Add("X-Custom", "value");

if (f.Send())
{
    string body = f.GetResponse();
    Console.WriteLine(body);
}
```

## What's Next

* [Basic Usage](basic-usage.md)
* [Call API (JSON)](call-api.md)
