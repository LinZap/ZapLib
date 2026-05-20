# Utility - Mirror

`Mirror` 是反射（reflection）相關的萬用工具。封裝了 `Type.GetProperties()`、跨組件搜尋類別、批次成員覆蓋等常見模式。

## Namespace

```csharp
using ZapLib.Utility;
```

## Members — 遍歷物件屬性

把任意物件當作 key-value 集合迭代：

```csharp
var user = new
{
    Id = 1,
    Name = "ZapLib",
    Since = new DateTime(2018, 1, 1)
};

foreach (DictionaryEntry m in Mirror.Members(user))
{
    Console.WriteLine($"{m.Key} = {m.Value}");
}
```

**輸出：**

```
Id = 1
Name = ZapLib
Since = 2018/1/1 上午 12:00:00
```

> **對 `IDictionary` 友善** — 傳入 `Dictionary<string, object>` 也能直接迭代。

## EachMembers — 帶型別轉換的迭代

每個成員都先轉成你要的型別：

```csharp
var data = new { name = "ZapLib", count = "42", price = "99.5" };

Mirror.EachMembers<string, string>(data, (key, value) =>
{
    Console.WriteLine($"{key}: {value}");
});
```

或要求數字型別 — 轉不過會給 `default(int)`：

```csharp
Mirror.EachMembers<string, int>(data, (key, value) =>
{
    // name → 0 (字串轉不出 int)
    // count → 42
    // price → 0 (含小數點)
    Console.WriteLine($"{key}: {value}");
});
```

## Assign — 物件合併

把多個物件的同名屬性**依序覆蓋**到目標物件上：

```csharp
public class UserSettings
{
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "light";
    public int FontSize { get; set; } = 14;
}

var defaults = new UserSettings();
var fromConfig = new { Language = "zh-TW", Theme = "dark" };
var fromQuery = new { FontSize = 16 };

UserSettings s = new UserSettings();
Mirror.Assign(ref s, defaults, fromConfig, fromQuery);

// s.Language = "zh-TW"  (從 fromConfig 覆蓋)
// s.Theme = "dark"       (從 fromConfig 覆蓋)
// s.FontSize = 16         (從 fromQuery 覆蓋)
```

> **覆蓋規則**：屬性名稱相同才會覆蓋，型別不同會走 `Cast.To()` 嘗試轉型，轉不過則保留原值。

## AssignValue — 單一屬性安全覆蓋

```csharp
public class Config
{
    public int Timeout { get; set; }
    public string Host { get; set; }
}

Config c = new Config();
Mirror.AssignValue(ref c, "Timeout", "30");      // "30" → 30, OK
Mirror.AssignValue(ref c, "Host", "localhost");  // OK
Mirror.AssignValue(ref c, "Unknown", "value");    // 屬性不存在，靜默忽略
Mirror.AssignValue(ref c, "Timeout", "xxx");     // 轉不過 int，靜默忽略
```

## GetClasses<T> — 全系統搜尋類別

找出當前 AppDomain 內所有衍生自 `T` 的類別：

```csharp
// 找所有 Hub 衍生類別
IEnumerable<Type> hubs = Mirror.GetClasses<Hub>();

foreach (var t in hubs)
{
    Console.WriteLine(t.FullName);
}
```

要包含 `T` 本身？傳 `include_self: true`：

```csharp
var all = Mirror.GetClasses<ApiController>(include_self: true);
```

## GetClasses<T> from Assembly

限定在特定 assembly 內搜尋（更快）：

```csharp
Assembly asm = Assembly.GetExecutingAssembly();
var controllers = Mirror.GetClasses<ApiController>(asm);
```

## GetCustomAttributes<T>

從 `PropertyInfo` 取出指定型別的 Attributes：

```csharp
public class ModelBook
{
    [SQLType(SqlDbType.NVarChar, Size = 50)]
    public string Name { get; set; }
}

PropertyInfo prop = typeof(ModelBook).GetProperty("Name");
var attrs = Mirror.GetCustomAttributes<ISQLTypeAttribute>(prop);

foreach (var a in attrs)
{
    Console.WriteLine($"{a.GetSQLType()} ({a.Size})");
    // 輸出：NVarChar (50)
}
```

> ZapLib 內部就是用這個機制在 `SQL` 類別中讀取 `[SQLType]` 並套用到 `SqlParameter`。

## Real-World Patterns

### Anonymous Object → Dictionary

```csharp
public static Dictionary<string, object> ToDictionary(object obj)
{
    var dict = new Dictionary<string, object>();
    foreach (var m in Mirror.Members(obj))
    {
        dict[m.Key.ToString()] = m.Value;
    }
    return dict;
}

var d = ToDictionary(new { id = 1, name = "x" });
```

### Plugin Discovery

掃描所有 plugin DLL 找實作某 interface 的類別：

```csharp
public interface IPlugin
{
    void Run();
}

var plugins = Mirror.GetClasses<IPlugin>()
    .Select(t => (IPlugin)Activator.CreateInstance(t));

foreach (var p in plugins) p.Run();
```

## Performance Notes

* **反射很慢**：每次 `GetProperties()` 都要走 metadata。熱路徑請快取
* `GetClasses<T>` 會掃**全部** assembly — 在大專案可能耗時。可改用 `GetClasses<T>(Assembly)` 限定範圍
* `Assign` 內部對每個物件都呼叫一次 `Members()`。對巨量物件迭代不適合
