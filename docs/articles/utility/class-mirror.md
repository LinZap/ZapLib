# Utility - ClassMirror

`ClassMirror` 用 `Type` 物件動態建立 instance、用方法名稱字串呼叫方法。把「反射呼叫」這件事的樣板程式碼壓到最小。

> 與 [`Mirror.GetClasses<T>()`](mirror.md) 搭配，可實現外掛式 / 動態載入的執行框架。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

### Create Instance from Type

```csharp
Type t = typeof(MyService);
ClassMirror cm = new ClassMirror(t);

if (cm.Instance == null)
{
    Console.WriteLine("無法建立：" + cm.ErrMsg);
}
```

### Create with Constructor Arguments

```csharp
public class MyService
{
    public MyService(string name, int level) { ... }
}

ClassMirror cm = new ClassMirror(typeof(MyService), "test", 5);
// 等價於 new MyService("test", 5);
```

### Invoke Method by Name

```csharp
ClassMirror cm = new ClassMirror(typeof(MyService));

MethodInfo run = cm["Run"];   // indexer 取得方法資訊
if (run != null)
{
    object result = run.Invoke(cm.Instance, new object[] { "input" });
}
```

`cm["MethodName"]` 找不到方法時回 `null` — 不會 throw。

## Complete Example

```csharp
public class Greeter
{
    public string Prefix { get; }

    public Greeter(string prefix)
    {
        Prefix = prefix;
    }

    public string SayHello(string name)
    {
        return $"{Prefix}, {name}!";
    }
}

// 動態建立並呼叫
Type t = typeof(Greeter);
ClassMirror cm = new ClassMirror(t, "Hello");

MethodInfo method = cm["SayHello"];
string result = (string)method.Invoke(cm.Instance, new object[] { "ZapLib" });

Console.WriteLine(result);   // "Hello, ZapLib!"
```

## Plugin Pattern

`ClassMirror` 的真實價值在搭配 [`DllLoader`](dll-loader.md) + [`Mirror.GetClasses<T>()`](mirror.md) 做外掛系統：

```csharp
// 1. 載入外部 DLL
Assembly plugin = DllLoader.Load(@"D:\plugins\MyPlugin.dll");

// 2. 找出實作 IPlugin 介面的所有類別
IEnumerable<Type> pluginTypes = Mirror.GetClasses<IPlugin>(plugin);

// 3. 對每個類別建立實例並呼叫
foreach (Type t in pluginTypes)
{
    ClassMirror cm = new ClassMirror(t);
    if (cm.Instance == null) continue;

    MethodInfo run = cm["Run"];
    if (run != null) run.Invoke(cm.Instance, null);
}
```

> 配合 [`DllLoader`](dll-loader.md) 可實現「丟個 DLL 進資料夾就會被執行」的外掛架構。

## Common Errors

`ErrMsg` 累積建構與方法查找時的錯誤：

| 錯誤訊息片段 | 原因 |
|---|---|
| `Constructor on type 'X' not found` | 沒對應的建構子，或參數型別不符 |
| `Type 'X' does not contain a constructor that takes...` | 給的 args 數量錯了 |
| `Object reference not set to an instance of an object` | `Type` 是 `null` |

## When to Use

* **外掛 / 模組化系統** — 透過設定檔載入未知類別
* **單元測試** — 動態測試多個 type 的某個共同方法
* **CLI tool / REPL** — 接收「字串型別名稱 + 字串方法名」執行

## When NOT to Use

* **編譯期已知的型別** — 直接 `new MyService(...)` 即可，反射有成本
* **熱路徑** — 反射呼叫比直接呼叫慢數十倍。建議快取 `MethodInfo` 或改用 `Delegate.CreateDelegate`

## See Also

* [Mirror](mirror.md) — `GetClasses<T>` 找出所有 type
* [DllLoader](dll-loader.md) — 動態載入外部 DLL
* [Dynamic Object](../dynamic-object.md) — 動態**建立新型別**（互補功能）
