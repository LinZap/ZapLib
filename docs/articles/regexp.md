# RegExp

`RegExp` 是 ZapLib 對 `System.Text.RegularExpressions.Regex` 的輕量封裝，提供 jQuery / JavaScript 風格的 `exec()` 介面 — 一行拿到所有比對結果與群組。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

```csharp
RegExp r = new RegExp(@"(\d{4})-(\d{2})-(\d{2})");
string[] result = r.Exec("今天是 2026-05-20，明天是 2026-05-21。");

foreach (string s in result)
{
    Console.WriteLine(s);
}
```

**輸出：**

```
2026-05-20
2026
05
20
2026-05-21
2026
05
21
```

> **回傳順序**：每個 `Match` 的完整匹配 + 各 group 依序列出，再進入下一個 `Match`。

## Case-Insensitive by Default

只傳 pattern 的建構子預設**不分大小寫**：

```csharp
RegExp r = new RegExp("hello");
string[] result = r.Exec("Hello World, HELLO again");

foreach (var s in result) Console.WriteLine(s);
```

**輸出：**

```
Hello
HELLO
```

## Custom Options

需要其他 `RegexOptions`？用第二個參數：

```csharp
RegExp r = new RegExp(@"^\d+$", RegexOptions.Multiline);
```

要**區分**大小寫，請明確指定 `RegexOptions.None`：

```csharp
RegExp r = new RegExp("Hello", RegexOptions.None);
string[] result = r.Exec("Hello World, HELLO again");
// 只會抓到一筆 "Hello"
```

## Common Patterns

### Extract Emails

```csharp
RegExp r = new RegExp(@"[\w\.\-]+@[\w\.\-]+\.\w+");
string[] emails = r.Exec("聯絡 alice@example.com 或 bob@test.org");
// → ["alice@example.com", "bob@test.org"]
```

### Extract Numbers

```csharp
RegExp r = new RegExp(@"\d+(?:\.\d+)?");
string[] nums = r.Exec("總金額：1,234.56，稅金 78");
// → ["1", "234.56", "78"]
```

### Capture Groups

```csharp
RegExp r = new RegExp(@"(\w+)=(\w+)");
string[] tokens = r.Exec("name=zaplib&version=2.5");
// → ["name=zaplib", "name", "zaplib", "version=2.5", "version", "2.5"]
```

> 對應到原生 `Match.Groups`：第 0 個是完整匹配，第 1 個之後是 capture group。

## When to Use Native Regex Instead

`RegExp` 適合**快速一次性匹配**。如果你需要：

* 替換（`Regex.Replace`）
* 取得 `Match` 的位置資訊（`Index`、`Length`）
* 重複使用同一個 pattern 提升效能（`RegexOptions.Compiled`）

請直接用原生 `System.Text.RegularExpressions.Regex`：

```csharp
using System.Text.RegularExpressions;

var regex = new Regex(@"\d+", RegexOptions.Compiled);
string result = regex.Replace("abc123def456", "***");
// → "abc***def***"
```
