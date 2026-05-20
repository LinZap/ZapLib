# JSON Reader

`JsonReader` 是 ZapLib 用串流方式解析 JSON 字串的工具。最大特色是 — **安全的深層索引存取**：對不存在的 key、null 值、型別不符通通不會 throw `NullReferenceException`，而是回傳 `default` 或你指定的預設值。

> **與 `JsonConvert.DeserializeObject<dynamic>` 的差別**：`dynamic` 對不存在的成員會 throw `RuntimeBinderException`。`JsonReader` 不會 — 適合「解析來源不可信、結構善變」的 JSON。

## Namespace

```csharp
using ZapLib.Json;
```

## Parse

```csharp
string json = @"{
    ""name"": ""ZapLib"",
    ""version"": 2.5,
    ""tags"": [""csharp"", ""lib""],
    ""author"": {
        ""name"": ""ZapLin"",
        ""since"": ""2018-01-01""
    }
}";

JsonReader reader = new JsonReader();
IJsonTuple root = reader.Parse(json);
```

`Parse` 回傳：

| JSON 根節點 | 回傳型別 |
|---|---|
| `{ ... }` | `ObjectTuple` |
| `[ ... ]` | `ArrayTuple` |
| `"abc"` / `123` / `true` / `null` | `ValueTuple` |
| 解析失敗 | `null` |

## Index Access

對 Object 用字串、對 Array 用數字：

```csharp
string name = root["name"].Value<string>();        // "ZapLib"
double version = root["version"].Value<double>();  // 2.5
string firstTag = root["tags"][0].Value<string>(); // "csharp"
string authorName = root["author"]["name"].Value<string>();  // "ZapLin"
```

## Safe Deep Access — 永不 NRE

關鍵特色：**深層存取不存在的路徑也不會炸**：

```csharp
// JSON 沒有 root.config.timeout.seconds 這條路徑
int timeout = root["config"]["timeout"]["seconds"].Value<int>(30);
// → 30 (default)，沒有 exception

// JSON 沒有 root.tags[99]
string tag = root["tags"][99].Value<string>("unknown");
// → "unknown"
```

> 內部機制：找不到的 key / 越界的 index，會回傳一個內含 `null` 的 `ValueTuple`。對 `ValueTuple` 再做索引，仍然回傳新的空 `ValueTuple`。所以**鏈式存取多深都安全**。

## Value Conversion

`Value<T>()` 用 [`Cast.To<T>`](utility/cast.md) 做型別轉換，轉不過時回 `default(T)` 或你指定的預設值：

```csharp
// 字串轉數字
int v = root["version"].Value<int>();   // 2 (因為原值是 2.5)

// 字串轉日期
DateTime since = root["author"]["since"].Value<DateTime>();
// → 2018-01-01

// 字串轉 Nullable
DateTime? expired = root["expired"].Value<DateTime?>();
// → null (因為原 JSON 沒有 expired)

// 帶預設值
string lang = root["language"].Value<string>("en");
// → "en"
```

## Raw Value

不需要轉型？直接拿底層 object：

```csharp
object raw = root["name"].Value();
// → "ZapLib" (typeof string)

// 對 Object/Array 拿到底層集合
Dictionary<string, IJsonTuple> dict = ((ObjectTuple)root).Value();
List<IJsonTuple> list = ((ArrayTuple)root["tags"]).Value();
```

## Iterate

`JsonReader` 沒有直接的 `foreach` 支援 — 需要先 cast 到 `ObjectTuple` / `ArrayTuple` 拿底層集合：

### 迭代 Object

```csharp
var obj = root["author"] as ObjectTuple;
if (obj != null)
{
    foreach (var kv in obj.Value())
    {
        Console.WriteLine($"{kv.Key} = {kv.Value.Value()}");
    }
}
```

### 迭代 Array

```csharp
var arr = root["tags"] as ArrayTuple;
if (arr != null)
{
    foreach (var item in arr.Value())
    {
        Console.WriteLine(item.Value<string>());
    }
}
```

## Parse Failure

無法解析的 JSON（語法錯誤）會回傳 `null`：

```csharp
IJsonTuple result = reader.Parse("this is not json");
if (result == null)
{
    Console.WriteLine("解析失敗");
}
```

## Real-World Pattern

處理外部 API 回應，欄位可能缺失：

```csharp
Fetch f = new Fetch("https://api.weather.com/forecast");
string json = f.Get(new { city = "Taipei" });

var reader = new JsonReader();
var data = reader.Parse(json);

if (data == null)
{
    return Error("Invalid response");
}

// 即使 API 改了結構或欄位缺失，這段也不會炸
string city = data["city"]["name"].Value<string>("Unknown");
double temp = data["current"]["temperature"]["celsius"].Value<double>(double.NaN);
string desc = data["current"]["description"]["zh-TW"].Value<string>(
                  data["current"]["description"]["en"].Value<string>("N/A")
              );

Console.WriteLine($"{city}: {temp}°C, {desc}");
```

> 這種「**rather than crash, return a sensible default**」的風格，特別適合接外部、不穩定的 API。

## When to Use What

| 情境 | 推薦 |
|---|---|
| JSON 結構穩定、有明確 schema | `JsonConvert.DeserializeObject<T>(json)` |
| 想用 IDE 補全、編譯期檢查 | 同上（強型別 model） |
| JSON 結構善變 / 欄位可能缺失 | **`JsonReader`** |
| 想最低成本快速 prototype | `JsonConvert.DeserializeObject<dynamic>` |
| 需要 LINQ / 進階查詢 | `JObject.Parse()` (Newtonsoft 的 LINQ to JSON) |

## See Also

* [Cast](utility/cast.md) — `Value<T>` 背後的型別轉換引擎
* [Fetch / Call API](fetch/call-api.md) — 直接拿 deserialized model
