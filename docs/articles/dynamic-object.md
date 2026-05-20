# Dynamic Object

`DynamicObject` 允許你在**執行期**才決定一個物件有哪些屬性 — 但它不是 `dynamic` 或 `ExpandoObject`，而是真正用 IL 編出一個 class。意思是：用反射（`GetProperties()`、`SetValue()`、`PropertyInfo`）能正常運作。

> 💡 典型用途：需要動態建立 model 餵給期望「真實 class」的 API — 例如 ORM、Web API model binding、CSV / Excel 動態欄位輸出等。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

三步驟：**定義類別 → 定義屬性 → 建立實例**：

```csharp
// 1. 定義一個新類別 "Person"
DynamicObject dyn = new DynamicObject("Person");

// 2. 加屬性
dyn.CreateProperty("Name", typeof(string), DefaultValue: "Unknown");
dyn.CreateProperty("Age", typeof(int), DefaultValue: 0);
dyn.CreateProperty("Birthday", typeof(DateTime?));

// 3. 建立實例
object person = dyn.CreateObject();
Type personType = dyn.CoreType;

// 4. 設值
dyn.SetProperty("Name", "ZapLin");
dyn.SetProperty("Age", 30);
dyn.SetProperty("Birthday", new DateTime(1994, 1, 1));

// 讀回來 — 必須透過反射
PropertyInfo nameProp = personType.GetProperty("Name");
Console.WriteLine(nameProp.GetValue(person));  // "ZapLin"
```

## Why Not Just Use dynamic / ExpandoObject?

| 需求 | `dynamic` / `ExpandoObject` | `DynamicObject` |
|---|---|---|
| 寫程式時直接 `obj.Name` 存取 | ✅ | ❌ 必須走反射 |
| 屬性可動態增減 | ✅ | ❌ 一旦 `CreateObject()` 後 schema 固定 |
| `obj.GetType().GetProperties()` 能列出屬性 | ❌ | ✅ |
| 餵給用反射綁定的 framework（如 ORM、Web API） | ❌ | ✅ |
| 序列化成 JSON | ✅ 但 schema 不固定 | ✅ schema 明確 |

**簡單口訣**：要被「以為自己在處理真實 class」的程式碼吃掉時，用 `DynamicObject`。

## Real-World Examples

### 動態餵 ZapLib SQL

ZapLib 的 `SQL.QuickQuery<T>` 用反射讀 `T` 的屬性。如果欄位是執行期才決定的（例如使用者自訂報表），就需要 `DynamicObject`：

```csharp
// 假設使用者選了 3 個欄位
string[] columns = new[] { "id", "name", "email" };

DynamicObject dyn = new DynamicObject("DynamicUser");
foreach (var col in columns)
{
    dyn.CreateProperty(col, typeof(string));
}
object proto = dyn.CreateObject();
Type t = dyn.CoreType;

// 動態呼叫 QuickQuery<T>
var method = typeof(SQL).GetMethod("QuickQuery").MakeGenericMethod(t);
SQL db = new SQL("DefaultConn");
object result = method.Invoke(db, new object[] {
    $"SELECT {string.Join(",", columns)} FROM Users",
    null,
    true
});

// result 是 DynamicUser[]，可以序列化
string json = JsonConvert.SerializeObject(result);
Console.WriteLine(json);
```

### 餵給 Web API Response

回應 client 一個結構不固定的物件，但要保留欄位名稱：

```csharp
DynamicObject dyn = new DynamicObject("Response");
dyn.CreateProperty("status", typeof(string));
dyn.CreateProperty("data", typeof(object));
dyn.CreateProperty("ts", typeof(long));

object resp = dyn.CreateObject();
dyn.SetProperty("status", "ok");
dyn.SetProperty("data", new { foo = "bar" });
dyn.SetProperty("ts", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

return api.GetResponse(resp);
// JSON 輸出：{"status":"ok","data":{"foo":"bar"},"ts":1747756800}
```

## API Reference

### `DynamicObject(string TypeName)`

建構子。指定要建立的類別名稱。**多次建立同名類別會在 `Reflection.Emit` 層級失敗** — 每次新建請給不同名字（含 GUID 為佳）：

```csharp
DynamicObject dyn = new DynamicObject($"AutoType_{Guid.NewGuid():N}");
```

### `CreateProperty(string PropertyName, Type PropertyType, object DefaultValue = null)`

新增一個屬性。**必須在 `CreateObject()` 之前呼叫**，之後再呼叫無效。

### `CreateObject()`

把所有已宣告的屬性編譯成實際類別並建立實例。回傳 `object`。同時填入 `Core`（實例）與 `CoreType`（型別）。

### `SetProperty(string PropertyName, object Value)`

設定屬性值。`CreateObject()` 之後才能呼叫。型別不符會 throw。

### `Core` / `CoreType`

`CreateObject()` 之後可讀：

* `Core`：實際的 instance（`object`）
* `CoreType`：實際的 `Type`，可拿來做反射

## Limitations & Caveats

* **不能加方法** — 只有屬性
* **不能繼承父類別** — 只能繼承 `object`
* **每個 `DynamicObject` 只能 `CreateObject()` 一次** — 之後屬性結構鎖定
* **效能**：`Reflection.Emit` 編譯類別有成本（毫秒級）。建議快取 `Core` / `CoreType`，不要每次請求都建一次
* **AssemblyBuilderAccess.Run** — 編出的類別只存在於記憶體，**無法存檔**

## Performance Cache Pattern

```csharp
public static class DynamicTypeCache
{
    private static readonly ConcurrentDictionary<string, Type> cache = new();

    public static Type GetOrCreate(string key, Action<DynamicObject> define)
    {
        return cache.GetOrAdd(key, k =>
        {
            var dyn = new DynamicObject($"Cached_{k}");
            define(dyn);
            dyn.CreateObject();
            return dyn.CoreType;
        });
    }
}

// 使用
Type t = DynamicTypeCache.GetOrCreate("UserBasic", dyn =>
{
    dyn.CreateProperty("Id", typeof(int));
    dyn.CreateProperty("Name", typeof(string));
});

object instance = Activator.CreateInstance(t);
```

## See Also

* [Mirror](utility/mirror.md) — 反射輔助工具
* [System.Reflection.Emit Namespace (Microsoft Docs)](https://learn.microsoft.com/dotnet/api/system.reflection.emit)
