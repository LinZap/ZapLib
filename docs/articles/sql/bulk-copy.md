# SQL - Bulk Copy

當你需要把**數千~數百萬筆**資料寫進 SQL Server 時，不要用迴圈跑 INSERT — 那會慢到無法接受。改用 `QuickBulkCopy`，底層是 SQL Server 原生的 `SqlBulkCopy`，速度可以快 10~100 倍。

## Namespace

```csharp
using ZapLib;
using System.Data;
```

## Basic Usage

```csharp
SQL db = new SQL("DefaultConn");

// 1. 準備 DataTable，欄位名稱要對應目標 table
DataTable dt = new DataTable();
dt.Columns.Add("name", typeof(string));
dt.Columns.Add("since", typeof(DateTime));

// 2. 塞資料
dt.Rows.Add("Book A", DateTime.Today);
dt.Rows.Add("Book B", DateTime.Today.AddDays(-1));
dt.Rows.Add("Book C", DateTime.Today.AddDays(-2));

// 3. 一鍵寫入
bool ok = db.QuickBulkCopy(dt, "Book");

if (!ok)
{
    Console.WriteLine("失敗：" + db.GetErrorMessage());
}
```

## How Column Mapping Works

`QuickBulkCopy` 用 **`DataTable.Columns[i].ColumnName` 對應 SQL Table 的欄位名**。所以：

* `DataTable` 欄位名 = `Book.name` → 對到 `Book.name`
* `DataTable` 欄位名 = `book_name` → SQL Server 找不到 `Book.book_name` → **失敗**

⚠️ **大小寫敏感性**取決於 SQL Server 的 Collation 設定。建議**完全一致**最保險。

## Build DataTable from List

實務上資料通常是 `List<T>`，需要先轉成 `DataTable`：

```csharp
public class BookRow
{
    public string Name { get; set; }
    public DateTime Since { get; set; }
}

public static DataTable ToDataTable<T>(IEnumerable<T> data)
{
    var dt = new DataTable();
    var props = typeof(T).GetProperties();

    foreach (var p in props)
    {
        var t = p.PropertyType;
        // Nullable<X> → X
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            t = Nullable.GetUnderlyingType(t);
        dt.Columns.Add(p.Name, t);
    }

    foreach (var item in data)
    {
        var row = dt.NewRow();
        foreach (var p in props)
            row[p.Name] = p.GetValue(item) ?? DBNull.Value;
        dt.Rows.Add(row);
    }

    return dt;
}

// 使用
List<BookRow> books = LoadBooksFromSomewhere();   // 假設有 10 萬筆
DataTable dt = ToDataTable(books);

SQL db = new SQL("DefaultConn");
bool ok = db.QuickBulkCopy(dt, "Book");
```

## With Triggers Enabled

`QuickBulkCopy` 內部用了 `SqlBulkCopyOptions.FireTriggers` — 也就是**目標 table 的 trigger 會被觸發**。如果你不想觸發 trigger（為了極致效能），就不能用 `QuickBulkCopy`，要自己寫：

```csharp
SQL db = new SQL("DefaultConn");
db.Connet();

if (db.IsConn)
{
    try
    {
        using (var bcp = new SqlBulkCopy(db.GetConnection()))
        {
            bcp.DestinationTableName = "Book";
            foreach (DataColumn col in dt.Columns)
                bcp.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            bcp.WriteToServer(dt);
        }
    }
    finally
    {
        db.Close();
    }
}
```

## Performance Tips

* **欄位數量越少越快** — DataTable 只放需要的欄位
* **避開 cluster index 大量隨機寫入** — 若可能，按 cluster key 排序後再 bulk copy
* **記憶體控管** — 數百萬筆建議分批，每批 50,000 ~ 100,000 筆
* **大批量時開 transaction**：失敗時不會有半截資料

```csharp
SQL db = new SQL("DefaultConn", transaction: true);

bool ok = db.QuickBulkCopy(dt, "Book");
// transaction commit/rollback 自動處理
```

## Error Handling

最常見錯誤是**欄位對應不上**或**欄位型別不符**：

```csharp
bool ok = db.QuickBulkCopy(dt, "Book");

if (!ok)
{
    Console.WriteLine(db.GetErrorMessage());
    // 範例輸出：
    // SQL BCP Error: Book
    // System.InvalidOperationException: The given ColumnName 'BookName' does not match up
    // with any column in data source.
}
```

排查順序：

1. **欄位名稱**是否完全對應？
2. **欄位型別**是否相容？（例如 DataTable 是 `string` 但 SQL 是 `int`）
3. **NOT NULL 欄位**是否都有值？（DBNull.Value 會違反 constraint）
4. **目標 table 是否存在 trigger**，trigger 內部執行錯誤？

## See Also

* [Basic Usage](basic-usage.md)
* [Modify](modify.md) — 小量資料的 Insert / Update / Delete
* [Transaction](transaction.md)
