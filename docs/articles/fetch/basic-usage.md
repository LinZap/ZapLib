# Fetch - Basic Usage

`Fetch` 是 ZapLib 的 HTTP 客戶端封裝。設計理念是把 .NET 原生的 `HttpClient` + `HttpClientHandler` + `HttpRequestMessage` 三件套包成一個物件，並提供 jQuery / Node.js 那種「一行送出請求」的友善介面。

## Namespace

```csharp
using ZapLib;
```

## HTTP GET

從一個 URL 抓回字串內容：

```csharp
// 目標網址
Fetch f = new Fetch("https://www.youtube.com/results");

// Query String
var qs = new
{
    search_query = "柴犬"
};

// 發出請求
string result = f.Get(qs);
Console.WriteLine(result);
```

**輸出：**

```html
<!DOCTYPE html><html lang="zh-Hant-TW" data-cast-api-enabled="true"><head>
<style name="www-roboto" >
@font-face{font-family:'Roboto';font-style:normal;font-weight:500;src:local
('Roboto Medium'),local('Roboto-Medium'),
url(//fonts.gstatic.com/s/roboto/v18/KFOlCnqEu92Fr1MmEU9fBBc9.ttf)
format('truetype');
...
```

## TLS 1.2 HTTPS

如果目標網址是 HTTPS、且伺服器停用了 TLS 1.0，需先指定開啟 TLS 1.2：

```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```

或者在 `App.config` / `Web.config` 中設定，`Fetch` 會在 `Send()` 時自動套用：

```xml
<appSettings>
  <add key="TLS12" value="true" />
</appSettings>
```

參考文章：[Windows 停用 TLS 1.0 之配套作業整理](https://blog.darkthread.net/blog/disable-tls-1-0-issues/)

## Download Resource

下載二進位資源（圖片、PDF、壓縮檔…）：

```csharp
Fetch f = new Fetch("https://static-s.aa-cdn.net/img/ios/1034197315/6294a01ff5937e26ca7539bea819db52");

// 取得二進位資料
byte[] img = f.GetBinary();

// 儲存成實體檔案
File.WriteAllBytes("dog.jpg", img);
```

> 預設下載大小上限為 25 MB。若要下載更大的檔案，請調整 `f.MaxDownloadSize`（單位：KB）。

## Fallback — 失敗處理

所有非 `2xx` 的回應，`Fetch` 一律回傳 `null`。**這是刻意的設計**，讓你可以用最少的判斷處理錯誤：

```csharp
Fetch f = new Fetch("https://httpbin.org/patch");
string result = f.Get();

if (result == null)
{
    Console.WriteLine($"失敗: {f.StatusCode} 原因: {f.GetResponse()}");
}
else
{
    Console.WriteLine(result);
}
```

**輸出：**

```html
失敗: 405 原因: <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">
<title>405 Method Not Allowed</title>
<h1>Method Not Allowed</h1>
<p>The method is not allowed for the requested URL.</p>
```

可用屬性與方法：

| 成員 | 說明 |
|---|---|
| `StatusCode` | HTTP 狀態碼（送出請求前為 `0`） |
| `GetResponse()` | 取得失敗回應的純文字內容 |
| `GetResponseHeaders()` | 取得所有回應 Header |
| `GetResponseHeader(key)` | 取得指定名稱的 Header |

## One Instance, One Request

⚠️ **每個 `Fetch` 物件只能發送一次請求**。如果你想用同樣的設定再打一次，請建立新物件：

```csharp
// ❌ 不行
Fetch f = new Fetch("https://api.example.com/users");
f.Get();
f.Get();   // 拋出 Exception: Every Fetch instance only send request once!

// ✅ 對
new Fetch("https://api.example.com/users").Get();
new Fetch("https://api.example.com/users").Get();
```

如果需要 long-living 共用一個底層 `HttpClient`，請直接用 `f.Client` 自行管理。

## Dispose

`Fetch` 實作 `IDisposable`，會釋放底層的 `HttpClient`、`Request`、`Response`：

```csharp
using (Fetch f = new Fetch("https://api.example.com/data"))
{
    string result = f.Get();
    // ...
}
```

## What's Next

* [Call API (JSON)](call-api.md) — 直接拿到型別化的 model
* [Advanced](advanced.md) — Header、Cookie、Proxy、檔案上傳、平台驗證
