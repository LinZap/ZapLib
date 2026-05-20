# SQL - Oracle

從 `v2.5.0-beta1` 開始，ZapLib 提供全新類別 `OracleSQL`，用於連線 Oracle 資料庫。它的設計理念與 [`SQL`](basic-usage.md) 一致 — 同樣的 `QuickQuery<T>` / `QuickDynamicQuery`、同樣的 `@name` 參數寫法 — 你只需要換 class 名稱即可。

## Highlights

* 底層採用 [Oracle.ManagedDataAccess](https://www.nuget.org/packages/Oracle.ManagedDataAccess) 純託管套件 — **部署機器不需要安裝 Oracle Client**
* SQL 中的參數寫法**沿用 `@name`** — 內部會自動轉成 Oracle 原生的 `:name` 並啟用 `BindByName`
* API 介面與 `SQL` 一致，已熟悉 ZapLib 的開發者零學習成本

> **目前支援範圍**：本版本為精簡版，僅先支援 `Query`、`QuickQuery<T>`、`QuickDynamicQuery` 三個常用查詢方法。其餘功能（BulkCopy、Stored Procedure、Transaction）將於後續版本擴充。

## Namespace

```csharp
using ZapLib;
```

## Configure Connection

在 `App.config` 或 `Web.config` 的 `connectionStrings` 中加入 Oracle 連線字串。**`providerName` 必須指定為 `Oracle.ManagedDataAccess.Client`**：

```xml
<connectionStrings>
  <add name="OracleConn"
       connectionString="User Id=zaplib;Password=zaplib123;Data Source=10.190.173.129:1521/FREEPDB1"
       providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>
```

## Initialize

`OracleSQL` 只提供一種建構子，傳入的字串可以是 `connectionStrings` 中的**名稱**或**完整連線字串**：

```csharp
// 以 connectionStrings 中的名稱
OracleSQL db = new OracleSQL("OracleConn");

// 或直接傳入完整連線字串
OracleSQL db2 = new OracleSQL(
    "User Id=zaplib;Password=zaplib123;Data Source=10.190.173.129:1521/FREEPDB1"
);
```

## Quick Query (Typed)

宣告與 Oracle 欄位對應的 model — 注意：Oracle 欄位習慣大寫，**屬性名稱比對不分大小寫**，所以兩種寫法都可以：

```csharp
// 寫法 1：依 Oracle 慣例大寫
public class HR_Row
{
    public string EMPN  { get; set; }
    public string ENAME { get; set; }
    public DateTime HIREDATE { get; set; }
}

// 寫法 2：用 C# 慣例
public class HrRow
{
    public string Empn  { get; set; }
    public string Ename { get; set; }
    public DateTime HireDate { get; set; }
}
```

```csharp
OracleSQL db = new OracleSQL("OracleConn");

HR_Row[] users = db.QuickQuery<HR_Row>(
    "SELECT * FROM HR_VIEW WHERE EMPN = @empn",
    new { empn = "625871" }
);

foreach (var u in users)
{
    Console.WriteLine($"{u.EMPN} {u.ENAME} {u.HIREDATE:yyyy-MM-dd}");
}
```

> 注意 SQL 寫的是 `@empn`，但實際送進 Oracle 時會被自動轉為 `:empn`。**你不需要關心這個轉換**。

## Quick Query (Dynamic)

不想宣告 model？直接拿 `dynamic[]`：

```csharp
dynamic[] rows = db.QuickDynamicQuery("SELECT * FROM HR_VIEW");

foreach (var row in rows)
{
    Console.WriteLine($"{row.EMPN} {row.ENAME}");
}
```

## Error Handling

與 `SQL` 一樣，失敗時回傳 `null`，錯誤訊息從 `GetErrorMessage()` 取得：

```csharp
HR_Row[] users = db.QuickQuery<HR_Row>("SELECT * FROM NOT_EXIST_TABLE");

if (users == null)
{
    Console.WriteLine("查詢失敗：" + db.GetErrorMessage());
}
```

## Manual Connection

需要更細的控制時，可手動開連線、跑 `Query()` 拿 `OracleDataReader`、自行 fetch：

```csharp
OracleSQL db = new OracleSQL("OracleConn");
db.Connet();

if (db.IsConn)
{
    using (var reader = db.Query("SELECT * FROM HR_VIEW WHERE EMPN = @empn",
                                  new { empn = "625871" }))
    {
        HR_Row[] data = db.fetch<HR_Row>(reader);
        // ... 自行處理
    }
    db.Close();
}
```

## Differences vs SQL

| 項目 | `SQL` | `OracleSQL` |
|---|---|---|
| 底層套件 | `System.Data.SqlClient` | `Oracle.ManagedDataAccess` |
| 部署需求 | — | 純託管，**免安裝 Oracle Client** |
| 建構子 | 3 種 | 1 種（僅接受連線字串或其名稱） |
| 參數寫法 | `@name` | `@name`（內部自動轉 `:name`） |
| Transaction | ✅ | 🚧 預留中 |
| Stored Procedure | ✅ | 🚧 規劃中 |
| Bulk Copy | ✅ | 🚧 規劃中 |

## See Also

* [SQL - Basic Usage](basic-usage.md) — `SQL` 類別的完整介紹
* [Changelog `2.5.0-beta1`](../changelog.md) — `OracleSQL` 的版本紀錄
