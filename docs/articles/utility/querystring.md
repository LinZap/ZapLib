# Utility - QueryString

`QueryString` 是兩個方向的 URL query string 工具：**物件序列化成字串**、**字串反序列化回物件**。

## Namespace

```csharp
using ZapLib.Utility;
```

## Parse — Object → Query String

把任意物件變成 URL query string：

```csharp
var data = new
{
    keyword = "ZapLib",
    page = 2,
    sort = "since desc"
};

string qs = QueryString.Parse(data);
Console.WriteLine(qs);
```

**輸出：**

```
keyword=ZapLib&page=2&sort=since%20desc
```

### URL Encoding 行為

預設會用 `Uri.EscapeUriString`（適合直接拼在 URL 後面）。如果想用 `HttpUtility.UrlEncode` 風格（百分號編碼非 ASCII 字元），在 config 加：

```xml
<appSettings>
  <add key="aspnet:DontUsePercentUUrlEncoding" value="true" />
</appSettings>
```

### Null / Empty 自動忽略

```csharp
var data = new
{
    a = "yes",
    b = (string)null,
    c = "",
    d = "  "
};

string qs = QueryString.Parse(data);
// → "a=yes"
// b, c, d 都被忽略
```

## Objectify<T> — Query String → Object

```csharp
public class SearchParam
{
    public string keyword { get; set; }
    public int page { get; set; }
    public string sort { get; set; }
}

string qs = "keyword=ZapLib&page=2&sort=since%20desc";
SearchParam p = QueryString.Objectify<SearchParam>(qs);

Console.WriteLine(p.keyword);  // ZapLib
Console.WriteLine(p.page);     // 2
Console.WriteLine(p.sort);     // since desc
```

> **欄位名稱比對不分大小寫**（透過 `Mirror.AssignValue`）。
>
> **型別自動轉換**：query 中的 `"2"` 會自動轉成 `int 2`，轉不過則保留屬性原值。

### 處理 URL Encoding

`Objectify<T>` 內部會先 `WebUtility.UrlDecode`，所以你**不需要**先手動解碼：

```csharp
string qs = "name=%E6%9F%B4%E7%8A%AC";   // "柴犬"
var p = QueryString.Objectify<MyModel>(qs);
// p.name = "柴犬"
```

## Real-World Patterns

### Fetch with Query String

`Fetch.Get()` 的 `qs` 參數內部就是用 `QueryString.Parse`：

```csharp
Fetch f = new Fetch("https://api.example.com/search");
string result = f.Get(new
{
    keyword = "ZapLib",
    page = 1,
    limit = 20
});
// 實際請求：GET https://api.example.com/search?keyword=ZapLib&page=1&limit=20
```

詳見 [Fetch / Basic Usage](../fetch/basic-usage.md)。

### Web API 收 Query String → 強型別

```csharp
[HttpGet]
public HttpResponseMessage Search()
{
    var api = new ExtApiHelper(this);
    string qs = Request.RequestUri.Query.TrimStart('?');

    SearchParam param = QueryString.Objectify<SearchParam>(qs);
    // ... 用 param.keyword、param.page 處理
}
```

> Web API 已內建 model binding，這個寫法只適合手動處理特殊情境（例如 query 名稱含 `[]` 或重複 key）。

### Object Round-Trip

```csharp
var original = new { id = 1, name = "ZapLib", active = "true" };

string qs = QueryString.Parse(original);
// "id=1&name=ZapLib&active=true"

dynamic restored = QueryString.Objectify<dynamic>(qs);
// 注意：dynamic 用 Objectify 會回 ExpandoObject 嗎？
// 答：不會。請定義具體 class 才能成功 Objectify
```

⚠️ **限制**：`Objectify<T>` 只支援**有公開無參建構子**的 class。`dynamic` / `ExpandoObject` 不支援。

## See Also

* [Fetch / Basic Usage](../fetch/basic-usage.md)
* [Cast](cast.md) — 內部的型別轉換引擎
* [Mirror](mirror.md) — 內部的反射引擎
