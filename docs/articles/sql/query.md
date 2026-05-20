# SQL - Query

進階查詢用法 — 手動連線、`SqlDataReader` 控制、與 `QuickQuery<T>` / `QuickDynamicQuery` 的選擇時機。

## Namespace

```csharp
using ZapLib;
```

## Quick vs Manual

ZapLib 的 SQL 查詢有「自動」與「手動」兩條路：

| 方法 | 連線 | 適用情境 |
|---|---|---|
| `QuickQuery<T>` | 自動開關 | 95% 的情境 — 一次性查詢 |
| `QuickDynamicQuery` | 自動開關 | 不想宣告 model 的一次性查詢 |
| `Query` | 手動 | 同一連線連續多次查詢、需 stream 大量資料 |

## QuickQuery<T> 詳解

完整簽章：

```csharp
public T[] QuickQuery<T>(string sql, object param = null, bool isfetchall = true)
```

### isfetchall 參數

預設 `true` — 取出所有資料。設為 `false` 時只取**第一筆**：

```csharp
// 只想知道某 user 是否存在
ModelUser[] u = db.QuickQuery<ModelUser>(
    "SELECT * FROM Users WHERE email = @email",
    new { email = "alice@example.com" },
    isfetchall: false
);

bool exists = u != null && u.Length > 0;
```

> 對大表查單筆時，`isfetchall: false` 可以省下記憶體與時間 — `SqlDataReader` 一拿到第一筆就 break，不會繼續讀。

### 欄位與屬性對應

* **不分大小寫** — `EMAIL`、`email`、`Email` 都會對到 `Email` 屬性
* **沒對到的欄位忽略** — SQL 多出來的欄位不影響
* **沒對到的屬性保留預設值**
* **`DBNull` 自動轉成 `null`**（Nullable 型別友善）

```csharp
public class ModelUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime? LastLogin { get; set; }   // Nullable，DBNull 會變 null
    public string DisplayName { get; set; }    // SQL 沒這欄，保持 null
}

ModelUser[] users = db.QuickQuery<ModelUser>("SELECT id, email, last_login FROM Users");
```

## QuickDynamicQuery 詳解

不想宣告 model 時的快速通道。回傳 `dynamic[]`，每個元素背後是 `ExpandoObject`：

```csharp
dynamic[] rows = db.QuickDynamicQuery(
    "SELECT TOP 5 id, name, price FROM Products ORDER BY price DESC"
);

foreach (dynamic row in rows)
{
    Console.WriteLine($"{row.id} {row.name} ${row.price}");
}
```

> ⚠️ `dynamic` 沒有 IDE 補全、沒有編譯期檢查。**錯字到 runtime 才會炸**。建議只在腳本 / 探索期使用。

## Manual Query

需要更精細控制？走手動三部曲：**`Connet()` → `Query()` → `Close()`**：

```csharp
SQL db = new SQL("DefaultConn");
db.Connet();   // 注意：方法名是 Connet，非 Connect (歷史拼字)

if (db.IsConn)
{
    try
    {
        using (SqlDataReader reader = db.Query(
            "SELECT * FROM BigTable WHERE created_at >= @from",
            new { from = DateTime.Today.AddDays(-30) }))
        {
            while (reader.Read())
            {
                // 一次處理一筆，不全部載入記憶體
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                ProcessRow(id, name);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("查詢失敗：" + ex.Message);
    }
    finally
    {
        db.Close();
    }
}
```

> 手動模式下**錯誤會 throw**，不像 `QuickXxx` 自動吞掉。需自己 `try/catch`。

## Reuse Connection for Multiple Queries

同一個 `SQL` 物件、同一個連線跑多次查詢：

```csharp
SQL db = new SQL("DefaultConn");
db.Connet();

if (db.IsConn)
{
    try
    {
        // 第一次查詢
        SqlDataReader r1 = db.Query("SELECT COUNT(*) FROM Users");
        r1.Read();
        int userCount = r1.GetInt32(0);
        r1.Close();

        // 第二次查詢（同一連線）
        SqlDataReader r2 = db.Query("SELECT COUNT(*) FROM Products");
        r2.Read();
        int productCount = r2.GetInt32(0);
        r2.Close();

        Console.WriteLine($"Users: {userCount}, Products: {productCount}");
    }
    finally
    {
        db.Close();
    }
}
```

## Manual Fetch to Model

拿到 `SqlDataReader` 後，可以用 `fetch<T>()` 一鍵綁定 model：

```csharp
db.Connet();
SqlDataReader reader = db.Query("SELECT * FROM Users");

ModelUser[] users = db.fetch<ModelUser>(reader);   // 全部
ModelUser firstUser = db.fetch<ModelUser>(reader, fetchAll: false)[0];   // 第一筆

reader.Close();
db.Close();
```

或用 `dynamicFetch()` 拿 dynamic：

```csharp
dynamic[] rows = db.dynamicFetch(reader);
```

## DB Always-On Read-Only Routing

如果你的環境啟用了 SQL Server AlwaysOn 高可用性架構，且想讓**唯讀查詢**自動走 ReadOnly secondary node：

```xml
<appSettings>
  <add key="EnableDBAlwaysOn" value="true" />
</appSettings>
```

```csharp
SQL db = new SQL("DefaultConn");
db.SQLReadOnly = true;

// 這個查詢會自動加上 ApplicationIntent=ReadOnly，路由到 secondary
ModelUser[] users = db.QuickQuery<ModelUser>("SELECT * FROM Users");
```

> **Fallback 機制**：如果 secondary 回傳空陣列（例如 secondary 還沒同步到資料），ZapLib 會自動切回 ReadWrite primary 再試一次。

## Trace Code

每個 `SQL` 物件有一組 `TraceCode`（GUID）— 寫進 `MyLog` 的所有訊息都會帶這個 code，方便追蹤同一個物件的所有操作：

```csharp
SQL db = new SQL("DefaultConn");
Console.WriteLine($"This SQL trace code: {db.TraceCode}");

db.QuickQuery<ModelUser>("SELECT * FROM Users");
// 日誌會出現：Exec sql: SELECT * FROM Users ... TraceCode: abc-123-...
```

## See Also

* [Basic Usage](basic-usage.md)
* [Modify](modify.md) — Insert / Update / Delete
* [Transaction](transaction.md)
