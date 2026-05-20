# Utility - Cast

`Cast` 是萬用型別轉換靜態類別。比起 `Convert.ChangeType()`，它**轉不過時不 throw**，而是回傳 `default(T)` 或你指定的預設值 — 寫起來更乾淨。

## Namespace

```csharp
using ZapLib.Utility;
```

## To<T> — 安全轉型

```csharp
int a = Cast.To<int>("42");                // 42
int b = Cast.To<int>("not a number");      // 0 (default)
int c = Cast.To<int>("not a number", 99);  // 99 (指定預設)

DateTime d = Cast.To<DateTime>("2026-05-20");          // 2026/5/20
double e = Cast.To<double>(null, 1.5);                 // 1.5
bool f = Cast.To<bool>("true");                        // true
```

> **與 `Convert.ChangeType` 的差別**：原生方法輸入 null / 不合法字串會丟 `InvalidCastException`、`FormatException`。`Cast.To<T>` 一律回 default 不 throw。

## To<T> for Nullable

```csharp
int? x = Cast.To<int?>("42");         // 42
int? y = Cast.To<int?>(null);          // null
int? z = Cast.To<int?>("not number");  // null（轉不過時依然安全）

DateTime? t = Cast.To<DateTime?>("2026-05-20");
```

## To with Type Parameter

不知道 T 是什麼編譯期型別？（例如反射場景）用非泛型多載：

```csharp
Type targetType = typeof(int);
object result = Cast.To("42", targetType);   // (object)42
```

## ToEnum

```csharp
public enum Status { Active, Inactive, Banned }

Status s1 = Cast.ToEnum<Status>("Active");      // Status.Active
Status s2 = Cast.ToEnum<Status>("Unknown");     // Status.Active (default(Status))
Status s3 = Cast.ToEnum<Status>("xxx", Status.Banned);  // Status.Banned

// 也支援用 enum 的數值
Status s4 = Cast.ToEnum<Status>("1");           // Status.Inactive
```

> 對於 `Enum.IsDefined` 為 `false` 的數值也會回傳預設值。安全。

## IsType<T> — 型別判斷

判斷物件是否為某類別、或衍生自某類別：

```csharp
object obj = "hello";

bool a = Cast.IsType<string>(obj);    // true
bool b = Cast.IsType<int>(obj);       // false
bool c = Cast.IsType<object>(obj);    // true (string 衍生自 object)
```

對 `null` 的處理：

```csharp
bool d = Cast.IsType<string>(null);   // true  (string 可為 null)
bool e = Cast.IsType<int>(null);      // false (int 不可為 null)
bool f = Cast.IsType<int?>(null);     // true  (int? 可為 null)
```

## IsType — Two Objects

判斷兩個物件是否為相同類別（或一個衍生自另一個）：

```csharp
string a = "hello";
string b = "world";
bool same = Cast.IsType(a, b);   // true

object c = 42;
object d = "hello";
bool diff = Cast.IsType(c, d);   // false
```

## CanBeNull

判斷 `Type` 是否可為 `null`：

```csharp
Cast.CanBeNull(typeof(string));    // true
Cast.CanBeNull(typeof(int));       // false
Cast.CanBeNull(typeof(int?));      // true
Cast.CanBeNull(typeof(DateTime));  // false
Cast.CanBeNull(typeof(DateTime?)); // true
```

實務常用在「決定要不要塞 `DBNull.Value` 還是 `null`」。

## Real-World Patterns

### 從 Query String 安全取參數

```csharp
var api = new ExtApiHelper(this);

int page = Cast.To<int>(api.GetQuery("page"), def_val: 1);
int limit = Cast.To<int>(api.GetQuery("limit"), def_val: 20);
Status status = Cast.ToEnum<Status>(api.GetQuery("status"), Status.Active);
```

### 從 Dictionary 安全取值

```csharp
Dictionary<string, object> data = ParseSomething();

string name = Cast.To<string>(data.TryGetValue("name", out var v1) ? v1 : null);
int age = Cast.To<int>(data.TryGetValue("age", out var v2) ? v2 : null, def_val: 0);
```

### 動態轉型到任意目標型別

```csharp
public static object SetValue(PropertyInfo prop, object rawValue)
{
    return Cast.To(rawValue, prop.PropertyType);
}
```

## When NOT to Use

* **金融計算 / 精準轉型** — 失敗應該要明確 throw，請用 `Convert.ChangeType` 或 `int.Parse`
* **解析 JSON** — 用 [Newtonsoft.Json](https://www.newtonsoft.com/json) 的 `JsonConvert.DeserializeObject`
* **大量轉型迴圈** — `Cast` 用反射 + try/catch，比直接 `(int)` 慢
