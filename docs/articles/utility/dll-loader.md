# Utility - DllLoader

`DllLoader` 是一個靜態類別，提供一個方法 — `Load(path)` — 從硬碟讀 DLL 檔，回傳 `Assembly`。

> **與 `Assembly.LoadFrom` / `Assembly.LoadFile` 的差別**：`DllLoader` 用 `File.OpenRead` + `Assembly.Load(byte[])` 的方式載入。意思是**載入後檔案不被鎖定**，可以一邊跑程式一邊覆蓋 DLL — 適合熱替換場景。

## Namespace

```csharp
using ZapLib.Utility;
```

## Basic Usage

```csharp
Assembly asm = DllLoader.Load(@"D:\plugins\MyPlugin.dll");

if (asm == null)
{
    Console.WriteLine("DLL 不存在");
    return;
}

// 列出組件內所有 public 類別
foreach (Type t in asm.GetExportedTypes())
{
    Console.WriteLine(t.FullName);
}
```

## Failure Modes

* **檔案不存在** → 回 `null`（不 throw）
* **檔案是合法 DLL 但 .NET 版本不相容** → throw `BadImageFormatException`
* **DLL 缺依賴** → throw `FileNotFoundException`（首次 reflect 時才發現）
* **DLL 已被另一個 process 鎖** → 依然能讀（因為用 `FileStream` 讀 bytes）

```csharp
try
{
    Assembly asm = DllLoader.Load(path);
    // ...
}
catch (BadImageFormatException ex)
{
    Console.WriteLine("DLL 格式錯誤：" + ex.Message);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("缺依賴：" + ex.Message);
}
```

## Locked File? Use DllLoader

對比 `Assembly.LoadFrom`：

```csharp
// ❌ LoadFrom 會鎖檔，跑這行後 plugin.dll 無法被刪除 / 覆蓋
Assembly a1 = Assembly.LoadFrom(@"D:\plugins\plugin.dll");

// ✅ DllLoader 把整個 DLL 讀進 byte[] 後再 load，原檔可被覆蓋
Assembly a2 = DllLoader.Load(@"D:\plugins\plugin.dll");
File.Delete(@"D:\plugins\plugin.dll");   // 可成功
```

> ⚠️ **但相對代價**：DLL 越大、記憶體吃越多。且**同一個 DLL 多次 Load 會在 AppDomain 中產生重複的型別**（不同 `Type` 物件，即使 FullName 相同）。

## Plugin Discovery Pattern

```csharp
public static List<IPlugin> LoadPlugins(string folder)
{
    var plugins = new List<IPlugin>();

    foreach (var dllPath in Directory.GetFiles(folder, "*.dll"))
    {
        Assembly asm = DllLoader.Load(dllPath);
        if (asm == null) continue;

        var pluginTypes = Mirror.GetClasses<IPlugin>(asm);
        foreach (Type t in pluginTypes)
        {
            var cm = new ClassMirror(t);
            if (cm.Instance != null)
                plugins.Add((IPlugin)cm.Instance);
        }
    }

    return plugins;
}

// 使用
var plugins = LoadPlugins(@"D:\app\plugins");
foreach (var p in plugins) p.Run();
```

## When to Use Built-in API Instead

| 需求 | 用什麼 |
|---|---|
| 載入 DLL 不需熱替換 | `Assembly.LoadFrom` |
| 完全卸載 DLL（含釋放記憶體） | `AssemblyLoadContext`（.NET Core 3+） |
| 動態編譯 C# code 成 DLL | `Roslyn` (`Microsoft.CodeAnalysis.CSharp`) |
| 跨 AppDomain 隔離載入 | `AppDomain.CreateDomain` |
| **熱替換、原始檔可被覆蓋** | **`DllLoader`** ✅ |

## See Also

* [ClassMirror](class-mirror.md) — 用 `Type` 建立 instance
* [Mirror](mirror.md) — `GetClasses<T>(Assembly)` 找特定介面的所有實作
