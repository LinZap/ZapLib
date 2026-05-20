# SQL - ExpParam (IN Clause Expansion)

C# 的 `SqlCommand.Parameters` **不支援陣列展開**到 `IN (...)` clause。例如：

```csharp
// ❌ 這樣不會動
db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE id IN (@ids)",
    new { ids = new[] { 1, 2, 3 } }
);
// SQL Server 會把 @ids 當成一個值，而不是 3 個
```

`ExpParam<T>` 解決這個問題 — 把 **單一參數展開成 `@var0`, `@var1`, `@var2`, ...** 並改寫 SQL。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

```csharp
int[] ids = new[] { 1, 5, 10 };

ModelBook[] books = db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE id IN (@ids)",
    new
    {
        ids = new ExpParam(ids)
    }
);
```

**實際送到 SQL Server 的 statement：**

```sql
SELECT * FROM Book WHERE id IN (@ids0, @ids1, @ids2)
```

並自動把 `@ids0=1, @ids1=5, @ids2=10` 綁定上去。

## 支援任何 IEnumerable

`ExpParam` 接受**任何 `IEnumerable`** — `int[]`、`List<string>`、`HashSet<Guid>` 通通可以：

```csharp
List<string> names = new List<string> { "Alice", "Bob", "Carol" };

ModelUser[] users = db.QuickQuery<ModelUser>(
    "SELECT * FROM Users WHERE name IN (@names)",
    new { names = new ExpParam(names) }
);
```

## Empty Array

如果傳入空集合，`ExpParam` 會綁一個 `DBNull` — `IN (NULL)` 永遠 false，這正是大部分情境想要的行為：

```csharp
int[] ids = new int[0];

ModelBook[] books = db.QuickQuery<ModelBook>(
    "SELECT * FROM Book WHERE id IN (@ids)",
    new { ids = new ExpParam(ids) }
);
// 等同：SELECT * FROM Book WHERE id IN (NULL) → 永遠 0 筆
```

> 如果你想要「空陣列 = 查所有」的語意，**請在呼叫前自行判斷**並改用不同 SQL。

## With Type Hint

如果參數的型別在 SQL Server 端是特定型別（例如 `NVARCHAR(50)`），可加 `[SQLType]`：

```csharp
public class QueryParam
{
    [SQLType(SqlDbType.NVarChar, Size = 50)]
    public ExpParam names { get; set; }
}

db.QuickQuery<ModelUser>(
    "SELECT * FROM Users WHERE name IN (@names)",
    new QueryParam
    {
        names = new ExpParam(new[] { "Alice", "Bob" })
    }
);
```

詳見 [SQL / Modify — Parameter Type Mapping](modify.md#parameter-type-mapping)。

## SQL Pattern Limitation

⚠️ **`ExpParam` 只能展開 `IN (@xxx)` 這個語法**。內部用的 regex 為：

```regex
\( *@paramName *\){1}
```

換句話說，**必須是被一對括號完整包住的 `@paramName`** 才會展開。不會展開以下情境：

```sql
-- ❌ 沒括號 — 不會展開
SELECT * FROM Book WHERE id = @ids OR id = @ids

-- ❌ 括號內還有其他 — 不會展開
SELECT * FROM Book WHERE id IN (@ids, 99)

-- ✅ 正確寫法
SELECT * FROM Book WHERE id IN (@ids)
```

## Use Cases

* **批次篩選**：`WHERE id IN (...)`、`WHERE status IN (...)`
* **動態 white-list**：`WHERE source NOT IN (@blacklist)`
* **跨表 JOIN 的批次過濾**：`WHERE u.id IN (@userIds)`

## When NOT to Use

* **超大陣列**（>2000 筆）— SQL Server `IN` clause 有參數數量上限。改用 Table-Valued Parameter (TVP) 或 `BulkCopy` 進臨時表後 JOIN
* **半結構化條件**（每筆條件不同）— 用 `OR` 拼接太醜，請改用 TVP
* **IN 子查詢**（`WHERE id IN (SELECT ...)`） — 跟 `ExpParam` 無關，直接寫子查詢就好

## See Also

* [Basic Usage](basic-usage.md)
* [Modify](modify.md) — `[SQLType]` 屬性的完整說明
