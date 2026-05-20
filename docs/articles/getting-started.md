# Getting Started

歡迎使用 ZapLib。本頁帶你在 5 分鐘內裝好套件、設定連線、跑出第一個範例。

> 📝 此頁為 Phase 1 骨架版本，後續 Phase 會擴充更多入門範例。

## 1. Install

**Package Manager**

```powershell
PM> Install-Package ZapLib
```

**.NET CLI**

```bash
dotnet add package ZapLib
```

詳細版本相容性請參考 [首頁的 System Requirement 表格](../index.md#system-requirement)。

## 2. Hello World

新建一個 Console 專案，加入：

```csharp
using System;
using ZapLib;

class Program
{
    static void Main()
    {
        Fetch f = new Fetch("https://httpbin.org/get");
        dynamic result = f.Get<dynamic>(new { name = "ZapLib" });
        Console.WriteLine(result.args.name);
    }
}
```

執行後應該看到：

```
ZapLib
```

## 3. 接下來

依使用情境挑選下一站：

* 想呼叫 HTTP API → [Fetch / Basic Usage](fetch/basic-usage.md)
* 想查 SQL Server → [SQL / Basic Usage](sql/basic-usage.md)
* 想查 Oracle → [SQL / Oracle](sql/oracle.md)
* 想了解全域設定 → [Global Config](global-config.md)
