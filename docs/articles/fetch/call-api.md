# Fetch - Call API (JSON)

呼叫 RESTful API 時，90% 的情境是「送 JSON、收 JSON」。`Fetch` 對這個情境提供泛型多載 — `Get<T>`、`Post<T>`、`Put<T>`、`Delete<T>`、`Patch<T>` — 自動處理 JSON 序列化、Accept Header、反序列化到你的 model。

## Namespace

```csharp
using ZapLib;
```

## GET → Strongly-Typed Model

定義 model：

```csharp
public class HttpBinResponse
{
    public Dictionary<string, string> args { get; set; }
    public Dictionary<string, string> headers { get; set; }
    public string url { get; set; }
}
```

呼叫並直接拿到綁定好的物件：

```csharp
Fetch f = new Fetch("https://httpbin.org/get");

HttpBinResponse data = f.Get<HttpBinResponse>(new { keyword = "ZapLib", lang = "zh-tw" });

Console.WriteLine(data.url);
Console.WriteLine(data.args["keyword"]);
```

**輸出：**

```
https://httpbin.org/get?keyword=ZapLib&lang=zh-tw
ZapLib
```

> Accept Header 會自動設為 `application/json`，回傳會用 [Newtonsoft.Json](https://www.newtonsoft.com/json) 反序列化。

## GET → Dynamic

不想宣告 model？拿 `dynamic`：

```csharp
Fetch f = new Fetch("https://httpbin.org/get");

dynamic data = f.Get<dynamic>(new { name = "ZapLib" });

Console.WriteLine(data.args.name);    // ZapLib
Console.WriteLine(data.url);          // https://httpbin.org/get?name=ZapLib
```

## POST JSON

POST 一個物件，body 會自動序列化成 JSON 並設定 `Content-Type: application/json`：

```csharp
Fetch f = new Fetch("https://httpbin.org/post");

var payload = new
{
    title  = "新文章",
    author = "ZapLin",
    tags   = new[] { "csharp", "zaplib" }
};

dynamic result = f.Post<dynamic>(payload);

Console.WriteLine(result.json.title);   // 新文章
```

## POST Form (x-www-form-urlencoded)

要送傳統表單而不是 JSON？把 `ContentType` 設成 `application/x-www-form-urlencoded`：

```csharp
Fetch f = new Fetch("https://httpbin.org/post");
f.ContentType = "application/x-www-form-urlencoded";

dynamic result = f.Post<dynamic>(new
{
    username = "linzap",
    password = "secret"
});

Console.WriteLine(result.form.username);
```

## PUT / DELETE / PATCH

所有 verb 都有兩個多載：**回字串**或**回 `T`**。用法一致：

```csharp
Fetch f1 = new Fetch("https://api.example.com/users/1");
dynamic updated = f1.Put<dynamic>(new { name = "ZapLin" });

Fetch f2 = new Fetch("https://api.example.com/users/1");
string text = f2.Delete();

Fetch f3 = new Fetch("https://api.example.com/users/1");
dynamic patched = f3.Patch<dynamic>(new { email = "new@example.com" });
```

## Upload File (multipart/form-data)

第三個參數 `files` 接受匿名物件，**屬性值是本機檔案路徑**。`Fetch` 會自動把 `ContentType` 切到 `multipart/form-data`：

```csharp
Fetch f = new Fetch("https://httpbin.org/post");

dynamic result = f.Post<dynamic>(
    data: new { description = "我的大頭貼" },
    qs:   null,
    files: new { avatar = @"C:\photos\me.jpg" }
);

Console.WriteLine(result.files.avatar);
```

> ⚠️ 注意：本機檔案不存在時會被靜默忽略（不會 throw）。請務必先用 `File.Exists()` 驗證。

## Error Handling

JSON 系列方法在失敗時回傳 `default(T)` — 即 `null`（reference type）或 `0` / `false`（value type）。判斷方式：

```csharp
Fetch f = new Fetch("https://httpbin.org/status/500");
HttpBinResponse data = f.Get<HttpBinResponse>();

if (data == null)
{
    Console.WriteLine($"Status: {f.StatusCode}");
    Console.WriteLine($"Body: {f.GetResponse()}");
}
```

> JSON 解析失敗（伺服器回 200 但 body 不是 valid JSON）會丟出 `JsonReaderException`，這時請用非泛型版本 `Get()` 拿原始字串再自行處理。

## What's Next

* [Basic Usage](basic-usage.md) — 純字串、二進位下載
* [Advanced](advanced.md) — Header、Cookie、Proxy、平台驗證
