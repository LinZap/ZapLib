# SQL - Transaction

當多個 SQL 操作必須**要嘛全成功、要嘛全失敗**時，需要 transaction（資料庫交易）。ZapLib 的 `SQL` 在建構時打開 `transaction: true` 即啟用。

## Namespace

```csharp
using ZapLib;
```

## Enable Transaction

任何 `SQL` 建構子的最後一個參數都是 `transaction`：

```csharp
SQL db = new SQL("DefaultConn", transaction: true);
```

啟用後：

* `Connet()` 開連線時會自動 `BeginTransaction()`
* 每個 `QuickXxx` 方法成功完成會自動 `Tran.Commit()`
* 任何一步丟出 exception 會自動 `Tran.Rollback()`

## Atomic Single Operation

⚠️ **`QuickXxx` 系列方法每次呼叫都會自動開關連線並 commit/rollback**。意思是「多筆 `QuickXxx` 仍各自為政」、**不是同一個 transaction**：

```csharp
SQL db = new SQL("DefaultConn", transaction: true);

// ❌ 這兩個 INSERT 不在同一個 transaction！
db.QuickQuery<dynamic>("INSERT INTO A (...) VALUES (...)", ...);
db.QuickQuery<dynamic>("INSERT INTO B (...) VALUES (...)", ...);  // 即使這句失敗，A 也已 commit
```

## Multi-Statement Transaction — Manual Mode

要讓多個 SQL 在**同一個 transaction** 中，必須走手動模式：

```csharp
SQL db = new SQL("DefaultConn", transaction: true);
db.Connet();

if (db.IsConn)
{
    try
    {
        // 第一個操作
        db.Cmd.CommandText = "INSERT INTO Orders (user_id, total) VALUES (@uid, @total)";
        db.Cmd.Parameters.Clear();
        db.Cmd.Parameters.AddWithValue("@uid", 100);
        db.Cmd.Parameters.AddWithValue("@total", 1500);
        db.Cmd.ExecuteNonQuery();

        // 第二個操作（同一個 transaction）
        db.Cmd.CommandText = "UPDATE Inventory SET stock = stock - 1 WHERE product_id = @pid";
        db.Cmd.Parameters.Clear();
        db.Cmd.Parameters.AddWithValue("@pid", 200);
        db.Cmd.ExecuteNonQuery();

        // 兩個都成功 → commit
        db.Tran.Commit();
    }
    catch (Exception ex)
    {
        // 任何一步失敗 → rollback
        db.Tran.Rollback();
        Console.WriteLine("交易失敗：" + ex.Message);
    }
    finally
    {
        db.Close();
    }
}
```

## Helper Pattern

把上面樣板包成一個 helper：

```csharp
public static bool RunInTransaction(string connName, Action<SQL> work)
{
    SQL db = new SQL(connName, transaction: true);
    db.Connet();

    if (!db.IsConn) return false;

    try
    {
        work(db);
        db.Tran.Commit();
        return true;
    }
    catch (Exception ex)
    {
        db.Tran.Rollback();
        new MyLog().Write("Transaction failed: " + ex.ToString());
        return false;
    }
    finally
    {
        db.Close();
    }
}

// 使用
bool ok = RunInTransaction("DefaultConn", db =>
{
    db.Cmd.CommandText = "INSERT INTO Orders ...";
    db.Cmd.ExecuteNonQuery();

    db.Cmd.CommandText = "UPDATE Inventory ...";
    db.Cmd.ExecuteNonQuery();
});
```

## Bulk Copy with Transaction

`QuickBulkCopy` 在 `transaction: true` 時，會在成功後自動 commit、失敗時自動 rollback：

```csharp
SQL db = new SQL("DefaultConn", transaction: true);

DataTable dt = BuildDataTable(largeData);
bool ok = db.QuickBulkCopy(dt, "TargetTable");

if (!ok)
{
    Console.WriteLine("批次寫入失敗，已 rollback");
}
```

詳見 [Bulk Copy](bulk-copy.md)。

## Inspecting the Transaction Object

需要直接操作 `SqlTransaction`？

```csharp
SQL db = new SQL("DefaultConn", transaction: true);
db.Connet();

SqlTransaction tran = db.Tran;
// 設定 IsolationLevel? 自訂 SavePoint?
tran.Save("BeforeCriticalStep");

// ... 跑 SQL ...

// 部分回滾到 save point
tran.Rollback("BeforeCriticalStep");
```

## See Also

* [Basic Usage](basic-usage.md)
* [Modify](modify.md)
* [Bulk Copy](bulk-copy.md)
