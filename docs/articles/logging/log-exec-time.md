# LogExecTime

`LogExecTime` 是一個極簡的計時工具 — `new` 出來就開始計時、呼叫 `Log()` 印出耗時。常用來測 SQL 查詢、外部 API 呼叫的效能。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

```csharp
LogExecTime t = new LogExecTime("Heavy Calculation");

// ... 做事
Thread.Sleep(1500);

t.Log();
```

**Log 檔輸出：**

```
[11:40:19] [Log Exec Time] Heavy Calculation
Takes 1.514 second
```

## Enable / Disable Globally

`Log()` 是否實際寫入，由 Config 控制：

```xml
<appSettings>
  <add key="LogExecTime" value="true" />
</appSettings>
```

* `true` → 寫入 `MyLog`
* `false` 或未設 → `Log()` 變成 no-op，**完全不寫**

> 設計理念：開發 / 除錯時開、Production 時關。**程式中的計時程式碼不需要為了開關到處包 `#if DEBUG`**。

## Read Time Manually

如果你想自己拿耗時、不依賴 config 開關：

```csharp
LogExecTime t = new LogExecTime("Inline Test");
DoSomething();
t.Log();   // 這行可能不寫檔，但 EndTime / DiffTime 一定會被算出來

TimeSpan elapsed = t.DiffTime;
double ms = elapsed.TotalMilliseconds;
Console.WriteLine($"耗時 {ms} ms");
```

可讀屬性：

| 屬性 | 說明 |
|---|---|
| `Name` | 行為名稱（建構時傳入） |
| `StartTime` | `new` 當下 |
| `EndTime` | `Log()` 被呼叫的當下 |
| `DiffTime` | `EndTime - StartTime` |

## SQL Auto-Tracing

`SQL` 與 `OracleSQL` 內部已經幫每個 query 包了 `LogExecTime` — 你**不需要手寫**：

```csharp
// 開 LogExecTime
<add key="LogExecTime" value="true" />

// 跑任何 SQL
SQL db = new SQL("DefaultConn");
db.QuickQuery<ModelBook>("SELECT * FROM Book");
```

Log 自動出現：

```
[11:40:19] [Log Exec Time] Exec sql: SELECT * FROM Book
Param: null
TraceCode: 7a5e2f...
Takes 0.014 second
```

## Pattern: 用 using 自動 Log

`LogExecTime` 本身**不實作 `IDisposable`**，但你可以包一層：

```csharp
public sealed class TimingScope : IDisposable
{
    private readonly LogExecTime _t;
    public TimingScope(string name) => _t = new LogExecTime(name);
    public void Dispose() => _t.Log();
}

// 使用
using (new TimingScope("Batch Import"))
{
    ImportFromCsv("data.csv");
}
// 離開 using → 自動 Log()
```

## When NOT to Use

* **微秒級效能量測** — `MyLog.Write` 本身就有 disk I/O 成本。請改用 `Stopwatch` + memory buffer
* **不想吃 ZapLib 的 Config 機制** — 自己用 `Stopwatch` 更輕量
* **想送到 APM / Application Insights** — 走專用 SDK，不要繞 `MyLog`

## See Also

* [MyLog](mylog.md) — `LogExecTime` 內部寫檔的工具
* [Global Config — LogExecTime](../global-config.md#logexectime)
