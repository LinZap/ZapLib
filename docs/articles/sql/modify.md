# SQL - Modify (Insert / Update / Delete)

INSERT / UPDATE / DELETE 在 ZapLib 中**沒有專用方法** — 它們和 SELECT 一樣，用 `QuickQuery<T>` 或 `Query` 執行即可。差別只在你是否關心回傳資料。

## Namespace

```csharp
using ZapLib;
```

## INSERT

最簡單的寫法 — 直接 `QuickQuery<dynamic>`（或任何 model），不在意回傳：

```csharp
SQL db = new SQL("DefaultConn");

db.QuickQuery<dynamic>(
    "INSERT INTO Book (name, since) VALUES (@name, @since)",
    new
    {
        name  = "Clean Code",
        since = DateTime.Now
    }
);
```

### INSERT + Return New ID

用 SQL Server 的 `OUTPUT INSERTED.id` 直接拿剛插入的 ID：

```csharp
public class InsertResult
{
    public int id { get; set; }
}

InsertResult[] result = db.QuickQuery<InsertResult>(
    @"INSERT INTO Book (name, since)
      OUTPUT INSERTED.id
      VALUES (@name, @since)",
    new { name = "Clean Code", since = DateTime.Now }
);

int newId = result[0].id;
```

### Batch INSERT

要塞數百~數萬筆？**不要 loop 跑 INSERT**。請改用 [Bulk Copy](bulk-copy.md)。

## UPDATE

```csharp
db.QuickQuery<dynamic>(
    "UPDATE Book SET name = @name WHERE id = @id",
    new
    {
        id   = 1,
        name = "Clean Code (2nd Edition)"
    }
);
```

### UPDATE + Get Affected Rows

`QuickQuery<T>` 不直接給你 affected rows。要拿到要走手動模式：

```csharp
db.Connet();
if (db.IsConn)
{
    try
    {
        db.Cmd.CommandText = "UPDATE Book SET name = @name WHERE id = @id";
        db.Cmd.Parameters.Clear();
        db.Cmd.Parameters.AddWithValue("@id", 1);
        db.Cmd.Parameters.AddWithValue("@name", "新名稱");

        int affected = db.Cmd.ExecuteNonQuery();
        Console.WriteLine($"更新了 {affected} 筆");
    }
    finally
    {
        db.Close();
    }
}
```

或用 `OUTPUT`：

```csharp
int[] updatedIds = db.QuickQuery<int>(
    @"UPDATE Book SET name = @name
      OUTPUT INSERTED.id
      WHERE since < @before",
    new { name = "Outdated", before = new DateTime(2020, 1, 1) }
);

Console.WriteLine($"更新了 {updatedIds.Length} 筆");
```

## DELETE

```csharp
db.QuickQuery<dynamic>(
    "DELETE FROM Book WHERE id = @id",
    new { id = 99 }
);
```

### Safe DELETE Pattern

刪除前**強烈建議用 SELECT 確認**：

```csharp
// 先確認影響範圍
ModelBook[] toDelete = db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE since < @before",
    new { before = new DateTime(2020, 1, 1) }
);

if (toDelete.Length > 100)
{
    Console.WriteLine($"⚠️ 即將刪除 {toDelete.Length} 筆，請手動確認");
    return;
}

// 確認 OK 再刪
db.QuickQuery<dynamic>(
    "DELETE FROM Book WHERE since < @before",
    new { before = new DateTime(2020, 1, 1) }
);
```

## Parameter Type Mapping

`QuickQuery` 預設用 `SqlCommand.Parameters.AddWithValue()` 自動推導型別。**99% 情況夠用**。但如果你遇到效能問題或需要精確控制（例如把 `string` 強制當 `NVarChar(50)` 而不是 `NVarChar(4000)`），用 `[SQLType]` Attribute：

```csharp
using ZapLib;

public class InsertParam
{
    public int id { get; set; }

    [SQLType(SqlDbType.NVarChar, Size = 50)]
    public string name { get; set; }

    [SQLType(SqlDbType.Date)]
    public DateTime since { get; set; }
}

db.QuickQuery<dynamic>(
    "INSERT INTO Book (id, name, since) VALUES (@id, @name, @since)",
    new InsertParam { id = 10, name = "Book A", since = DateTime.Today }
);
```

## Error Handling

`QuickQuery` 失敗時回傳 `null`，錯誤訊息看 `GetErrorMessage()`：

```csharp
var result = db.QuickQuery<dynamic>(
    "INSERT INTO Book (id, name) VALUES (@id, @name)",
    new { id = 1, name = "Duplicate" }   // 假設 id=1 已存在會 PK 衝突
);

if (result == null)
{
    Console.WriteLine("失敗：" + db.GetErrorMessage());
}
```

## See Also

* [Basic Usage](basic-usage.md)
* [Transaction](transaction.md) — 多筆操作的原子性
* [Bulk Copy](bulk-copy.md) — 大量資料寫入
