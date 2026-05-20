# SQL - Basic Usage

`SQL` 類別封裝了 SQL Server 的常見操作 — 連線、查詢、預存程序、Bulk Copy、Transaction — 並提供 `QuickXxx` 系列方法，幫你自動處理「開連線 → 執行 → 關連線」的樣板程式碼。

## Namespace

```csharp
using ZapLib;
```

## Initialize

`SQL` 提供三種建構子，依使用情境選擇即可：

### 從 .config 自動讀取連線資訊

如果 `App.config` / `Web.config` 中已寫好 `DBHost`、`DBName`、`DBAct`、`DBPwd`，可不傳任何參數：

```xml
<appSettings>
  <add key="DBHost" value="10.0.0.1" />
  <add key="DBName" value="MyDatabase" />
  <add key="DBAct"  value="sa" />
  <add key="DBPwd"  value="P@ssw0rd" />
</appSettings>
```

```csharp
SQL db = new SQL();
```

### 直接傳入連線資訊

```csharp
SQL db = new SQL(
    dbHost: "10.0.0.1",
    dbName: "MyDatabase",
    dbAct:  "sa",
    dbPwd:  "P@ssw0rd"
);
```

### 從 connectionStrings 或完整連線字串

```xml
<connectionStrings>
  <add name="DefaultConn"
       connectionString="Server=10.0.0.1;Database=MyDatabase;User ID=sa;Password=P@ssw0rd" />
</connectionStrings>
```

```csharp
// 以名稱
SQL db = new SQL("DefaultConn");

// 或直接給字串
SQL db2 = new SQL("Server=10.0.0.1;Database=MyDatabase;User ID=sa;Password=P@ssw0rd");
```

## Sample Schema

以下章節的範例都以這張 `Book` 表為例：

| id | name        | since               |
| -- | ----------- | ------------------- |
| 1  | 程式設計寶典       | 1999-01-01 00:00:00 |
| 2  | 大家都來寫程式     | 1999-01-01 00:00:00 |

### Declare Data Model

定義一個能夠承接資料的模型類別，**屬性名稱需與資料表欄位對齊**（不分大小寫）：

```csharp
public class ModelBook
{
    public int id { get; set; }
    public string name { get; set; }
    public DateTime since { get; set; }
}
```

## Get All Data

`QuickQuery<T>` 會自動開連線、執行、把結果倒進 `T[]`、最後關連線。**這是最常用的查詢方法。**

```csharp
SQL db = new SQL("DefaultConn");

ModelBook[] books = db.QuickQuery<ModelBook>("SELECT * FROM Book");

foreach (var book in books)
{
    Console.WriteLine($"{book.id} {book.name} {book.since:yyyy-MM-dd}");
}
```

**輸出：**

```
1 程式設計寶典 1999-01-01
2 大家都來寫程式 1999-01-01
```

## Get One Row

只想拿第一筆？把第三個參數 `isfetchall` 設成 `false`：

```csharp
ModelBook[] result = db.QuickQuery<ModelBook>("SELECT * FROM Book", null, isfetchall: false);
ModelBook book = result[0];
```

## Parameterized Query

**絕對不要用字串拼接 SQL**。`QuickQuery<T>` 的第二個參數接受匿名物件，屬性名對應 SQL 中的 `@param`：

```csharp
ModelBook[] books = db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE id = @id AND since >= @from",
    new {
        id = 1,
        from = new DateTime(1999, 1, 1)
    }
);
```

## IN Clause with Arrays

C# 的 `SqlCommand.Parameters` 預設**不支援陣列展開**到 `IN (...)`。ZapLib 提供 `ExpParam` 解決：

```csharp
int[] ids = new[] { 1, 5, 10 };

ModelBook[] books = db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE id IN (@ids)",
    new { ids = new ExpParam(ids) }
);
// 實際 SQL：SELECT * FROM Book WHERE id IN (@ids0, @ids1, @ids2)
```

詳見 [SQL / ExpParam](exp-param.md)。

## Get Without Model — Dynamic

懶得宣告 model？用 `QuickDynamicQuery` 拿 `dynamic[]`：

```csharp
dynamic[] rows = db.QuickDynamicQuery("SELECT id, name FROM Book");

foreach (var row in rows)
{
    Console.WriteLine($"{row.id} {row.name}");
}
```

> **何時用哪一個？** 真實專案建議用 `QuickQuery<T>` — 有型別檢查、IDE 補全友善。`QuickDynamicQuery` 適合一次性腳本或欄位會浮動的查詢。

## Error Handling

`QuickXxx` 方法在失敗時**不會 throw**，而是回傳 `null`。錯誤訊息可從 `GetErrorMessage()` 取得：

```csharp
ModelBook[] books = db.QuickQuery<ModelBook>("SELECT * FROM NotExistTable");

if (books == null)
{
    Console.WriteLine("查詢失敗：" + db.GetErrorMessage());
}
```

**輸出：**

```
查詢失敗：SQL Error:SELECT * FROM NotExistTable para:null
System.Data.SqlClient.SqlException: Invalid object name 'NotExistTable'.
   at ...
```

## Connection Options

`SQL` 物件建構後、第一次連線前，可調整下列屬性：

| 屬性 | 預設 | 說明 |
|---|---|---|
| `Timeout` | `30` | 連線逾時秒數 |
| `Encrypt` | `false` | 是否使用 SSL 加密傳輸 |
| `TrustServerCertificate` | `false` | 是否信任伺服器憑證 |
| `MultiSubnetFailover` | `false` | 跨子網路的可用性群組 |
| `ApplicationIntent` | `"ReadWrite"` | 工作負載類型 |
| `EnableDBAlwaysOn` | `false` | 啟用 AlwaysOn 唯讀路由 |

範例：

```csharp
SQL db = new SQL("DefaultConn");
db.Timeout = 60;
db.Encrypt = true;
db.TrustServerCertificate = true;

ModelBook[] books = db.QuickQuery<ModelBook>("SELECT * FROM Book");
```

## What's Next

* [Query](query.md) — 進階查詢、`SqlDataReader` 手動操作
* [Modify](modify.md) — Insert / Update / Delete
* [Transaction](transaction.md) — 交易控制
* [Bulk Copy](bulk-copy.md) — 大量資料寫入
* [Stored Procedure](stored-procedure.md) — 呼叫預存程序
* [ExpParam](exp-param.md) — `IN (...)` 陣列展開
* [Oracle](oracle.md) — 連線 Oracle 資料庫
