# SQL - Stored Procedure

呼叫 SQL Server 預存程序（Stored Procedure，SP），並把 OUTPUT 參數綁定到 C# model。

## Namespace

```csharp
using ZapLib;
```

## QuickExec<T>

`QuickExec<T>` 是呼叫 SP 的主要方法。會自動：

1. 開連線
2. 把 SP 的 OUTPUT 參數綁定到 `T` 的屬性
3. 執行 SP
4. 把 OUTPUT 值寫回 `T` 物件
5. 關連線

### Example SP

假設資料庫有這支 SP：

```sql
CREATE PROCEDURE dbo.CreateOrder
    @userId   INT,
    @amount   MONEY,
    @orderId  INT      OUTPUT,
    @status   NVARCHAR(20) OUTPUT
AS
BEGIN
    INSERT INTO Orders (user_id, amount) VALUES (@userId, @amount);
    SET @orderId = SCOPE_IDENTITY();
    SET @status = 'created';
END
```

### 對應的 C# Model

```csharp
public class CreateOrderResult
{
    public int orderId { get; set; }      // SP 的 @orderId OUTPUT
    public string status { get; set; }    // SP 的 @status OUTPUT
}
```

> **規則**：`T` 的**每個屬性**都會被當成 OUTPUT 參數。屬性名需與 SP 的 OUTPUT 參數名一致（不分大小寫）。

### 呼叫

```csharp
SQL db = new SQL("DefaultConn");

CreateOrderResult result = db.QuickExec<CreateOrderResult>(
    "dbo.CreateOrder",
    new
    {
        userId = 100,
        amount = 1500
    }
);

Console.WriteLine($"新訂單 ID: {result.orderId}, 狀態: {result.status}");
```

## INPUT-Only SP

SP 沒有 OUTPUT、只想觸發執行？用 `dynamic` 當 T，傳空物件即可：

```csharp
public class EmptyResult { }

db.QuickExec<EmptyResult>(
    "dbo.PurgeOldOrders",
    new { days = 90 }
);
```

或乾脆走 `QuickQuery<T>` — 對 SP 一樣有效（如果 SP 有 `SELECT` 語句）：

```csharp
public class StatusRow
{
    public int total { get; set; }
    public int deleted { get; set; }
}

StatusRow[] rows = db.QuickQuery<StatusRow>(
    "EXEC dbo.PurgeOldOrders @days = @days",
    new { days = 90 }
);
```

## Specify OUTPUT Parameter Size

當 OUTPUT 是 `NVARCHAR` / `VARCHAR` 時，**預設大小可能太小或太大**。用 `[SQLType]` 精確指定：

```csharp
using ZapLib;

public class CreateOrderResult
{
    public int orderId { get; set; }

    [SQLType(SqlDbType.NVarChar, Size = 20)]
    public string status { get; set; }

    [SQLType(SqlDbType.NVarChar, Size = 200)]
    public string message { get; set; }
}
```

> 不指定時 `NVarChar` 預設給 4000，**通常沒問題但會浪費記憶體**。

## Multiple Result Sets

`QuickExec<T>` 只處理 OUTPUT 參數，**不處理 SELECT 結果集**。如果 SP 同時要回 OUTPUT 又要回多個 result set，得走手動模式：

```csharp
SQL db = new SQL("DefaultConn");
db.Connect();

if (db.IsConn)
{
    try
    {
        db.Cmd.CommandText = "dbo.ComplexReport";
        db.Cmd.CommandType = CommandType.StoredProcedure;
        db.Cmd.Parameters.Clear();
        db.Cmd.Parameters.AddWithValue("@year", 2026);

        var totalParam = new SqlParameter("@total", SqlDbType.Int) { Direction = ParameterDirection.Output };
        db.Cmd.Parameters.Add(totalParam);

        using (SqlDataReader reader = db.Cmd.ExecuteReader())
        {
            // 第一個 result set
            while (reader.Read())
            {
                // ...
            }
            reader.NextResult();

            // 第二個 result set
            while (reader.Read())
            {
                // ...
            }
        }

        // OUTPUT 在 reader 關閉後才可讀
        int total = (int)totalParam.Value;
    }
    finally
    {
        db.Close();
    }
}
```

## Error Handling

```csharp
CreateOrderResult result = db.QuickExec<CreateOrderResult>(
    "dbo.CreateOrder",
    new { userId = 100, amount = 1500 }
);

// QuickExec<T> 失敗時回傳 default(T) — 通常是 null
if (result == null)
{
    Console.WriteLine("SP 執行失敗：" + db.GetErrorMessage());
}
```

## ⚠️ 已棄用的 Exec<T>

```csharp
[Obsolete("Exec<T> 即將棄用，請改用 QuickExec<T> 取代")]
public T Exec<T>(string sql, object param = null)
```

舊版的 `Exec<T>` 需要呼叫端自行管理連線，**已標記為棄用**。新程式碼請一律用 `QuickExec<T>`。

## See Also

* [Basic Usage](basic-usage.md)
* [Query](query.md)
