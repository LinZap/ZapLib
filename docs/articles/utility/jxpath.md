# Utility - JXPath

`JXPath` 用類似 XPath 的字串路徑從 JSON 字串或 `dynamic` 物件中取值。比 `Newtonsoft.Json` 的 `SelectToken` 簡潔、對深層存取友善。

> 與 [JsonReader](../json-reader.md) 的差別：`JsonReader` 用**索引子鏈式存取**（`root["a"]["b"]`），`JXPath` 用**單一字串路徑**（`"a/b"`）。後者適合「路徑由外部設定檔給定」的場景。

## Namespace

```csharp
using ZapLib.Utility;
```

## Basic Usage

### From JSON String

```csharp
string json = @"{
    ""user"": {
        ""name"": ""ZapLin"",
        ""age"": 30,
        ""tags"": [""csharp"", ""dotnet"", ""sql""]
    }
}";

object name = JXPath.GetValue(json, "user/name");
object age = JXPath.GetValue(json, "user/age");
object firstTag = JXPath.GetValue(json, "user/tags[0]");
object lastTag = JXPath.GetValue(json, "user/tags[2]");

Console.WriteLine(name);     // ZapLin
Console.WriteLine(age);      // 30
Console.WriteLine(firstTag); // csharp
Console.WriteLine(lastTag);  // sql
```

### From Dynamic

如果你已經 parse 過 JSON 拿到 `dynamic`：

```csharp
dynamic data = JObject.Parse(json);
object name = JXPath.GetValue(data, "user/name");
```

## Path Syntax

| 寫法 | 意義 |
|---|---|
| `a/b/c` | 對應 `data.a.b.c` |
| `a/b[0]` | 對應 `data.a.b[0]` |
| `a/b[3]/c` | 對應 `data.a.b[3].c` |

* 階層用 `/` 分隔
* 陣列索引用 `[數字]`
* **不支援**：屬性過濾（`[@attr='x']`）、萬用字元（`*`）、條件式 — 這不是真正的 XPath

## Failure Returns Null

路徑寫錯或不存在的 key：

```csharp
object x = JXPath.GetValue(json, "user/nonexistent/field");
// → null

object y = JXPath.GetValue(json, "user/tags[99]");
// → null
```

> 不會 throw、不會 NRE。失敗的細節會寫到 `Trace.WriteLine`，可在 Visual Studio Output 視窗看到。

## ParseXPath

如果你想自己處理路徑邏輯：

```csharp
List<(string, int)> parts = JXPath.ParseXPath("user/tags[0]/name");
// → [ ("user", -1), ("tags", 0), ("name", -1) ]
```

* `(key, -1)` → 純物件存取
* `(key, idx)` → 取出物件後再取 array 第 `idx` 個元素

## Type Casting

`GetValue` 回傳 `object`。實際內容可能是 `JValue`、`JObject`、`JArray`，**不會自動轉成 string / int**：

```csharp
object raw = JXPath.GetValue(json, "user/age");
// raw 是 JValue，不是 int

// 要拿原生型別
int age = Convert.ToInt32(raw);
// 或借用 Cast
int age2 = Cast.To<int>(raw);
// 或用 JValue 自己的轉型
int age3 = ((JValue)raw).Value<int>();
```

## Real-World Pattern: 設定驅動的欄位映射

```xml
<appSettings>
  <add key="UserName_Path"  value="result/data/user/displayName" />
  <add key="UserEmail_Path" value="result/data/user/contact/email" />
</appSettings>
```

```csharp
Fetch f = new Fetch("https://api.example.com/me");
string json = f.Get();

string name = JXPath.GetValue(json, Config.Get("UserName_Path"))?.ToString();
string email = JXPath.GetValue(json, Config.Get("UserEmail_Path"))?.ToString();
```

> 這種「**路徑寫在 config，程式不用改**」的模式，適合對接多個 schema 略有差異的外部 API。

## See Also

* [JsonReader](../json-reader.md) — 對同樣場景的另一種解法（鏈式存取）
* [Newtonsoft.Json SelectToken](https://www.newtonsoft.com/json/help/html/SelectToken.htm) — 原生支援完整 JSON Path 語法
