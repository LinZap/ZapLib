# ZapLib

> 受 jQuery 與 Node.js 啟發，**ZapLib** 是一個輕量的 C# 工具庫，協助開發者用最少的程式碼完成複雜任務 — 從 HTTP 請求、SQL Server / Oracle 查詢、SMTP 寄信、正規表達式到 .NET Web API 擴充，皆可一氣呵成。

[![NuGet](https://img.shields.io/nuget/v/ZapLib.svg)](https://www.nuget.org/packages/ZapLib/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/LinZap/ZapLib/blob/master/LICENSE.md)

## Install

**Package Manager**

```powershell
PM> Install-Package ZapLib
```

**.NET CLI**

```bash
dotnet add package ZapLib
```

## System Requirement

| ZapLib 版本 | .NET Framework |
|-----------|----------------|
| ≥ `v2.1.0` | 4.7.2 |
| ≥ `v1.23.0` | 4.7.2 |
| ≥ `v1.12.0` | 4.5 |
| ≤ `v1.10.0` | 4.0 |

## Hello, ZapLib

只要三行程式碼，呼叫一個 HTTP API 並取回 JSON：

```csharp
using ZapLib;

Fetch f = new Fetch("https://httpbin.org/get");
dynamic result = f.Get<dynamic>(new { name = "ZapLib" });

Console.WriteLine(result.args.name);   // ZapLib
```

或者用 3 行查資料庫：

```csharp
using ZapLib;

SQL db = new SQL("DefaultConn");
dynamic[] rows = db.QuickDynamicQuery("SELECT * FROM Book WHERE id = @id", new { id = 1 });

Console.WriteLine(rows[0].name);
```

## 如何閱讀本站

本文件分成兩大區塊：

* **[Articles](articles/getting-started.md)** — 主題式教學，從安裝、設定到各模組的真實情境範例
* **[API Reference](api/index.md)** — 由 source code 自動產生的完整型別 / 方法 / 屬性字典

如果你是第一次使用 ZapLib，建議從 [Getting Started](articles/getting-started.md) 開始。

## .NET WebAPI 注意事項

從 `v1.16.0` 起 ZapLib 內建 SignalR。若你的 WebAPI 專案**沒有**使用 SignalR，請在 `Web.config` 加入：

```xml
<appSettings>
  <add key="owin:AutomaticAppStartup" value="false" />
</appSettings>
```

否則啟動時會自動載入 OWIN，可能與既有設定衝突。

## License

ZapLib 以 [MIT License](https://github.com/LinZap/ZapLib/blob/master/LICENSE.md) 釋出。
文件部分以創用 CC 姓名標示-非商業性-相同方式分享 3.0 台灣 授權條款釋出。
