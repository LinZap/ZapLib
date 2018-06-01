# [ZapLib](https://www.nuget.org/packages/ZapLib/)

ZapLib 受到 jQuery, Node.js 的靈感啟發，在 C# 也提供一套非常輕巧的函式庫，開發人員能夠快速完成複雜的功能。無論您需要 Http 請求、SQL Server 的查詢、.NET Web Api 的擴充函式、SMTP 寄信、Regular Expression... 等，都能用簡單的程式碼來完成它！

## Installation

**Package Manager**

```
PM> Install-Package ZapLib -Version 1.17.0
```

## System requirement

* `v1.10.0` 以前的版本支援 .NET Framework 4.0 以上
* `v1.12.0` 開始的版本僅支援 .NET Framework 4.5 以上
* `v1.16.0` 開始的版本包含 SignalR

## API Reference

* Fetch：
* SQL：
* Extension Web Api Helper：
* Mailer：
* Regular Expression：
* Query String：
* Config：
* Log：
* ApiControllerSignalR：
* 實務案例(一)：
* 實務案例(二)：


## ChangeLog
改版紀錄

### `v1.18.0`

新增了 `ApiHelper.getStreamResponse` 用從伺服器中抓取檔案並以串流方式回應，使用範例如下：
   
**`getStreamResponse`**  
  
| arg  |  type | required | description |
| --------  | -------- | -------- |  -------- |
| filePath     |   string     | Y | 檔案實體路徑 |
| name         |   string     | N | 檔案名稱，如果沒有給予，則會以亂數命名 |
| type         |   string     | N | 檔案 MIME TYPE，如果沒有給予，則會以預設 `application/octet-stream` |
| disposition  |   string     | N | 串流方式，預設為 `attachment` 會直接下載檔案，如果要在瀏覽器中顯示，例如圖片，可以設定成 `inline` |
  
> 注意：`disposition` 如果設置為 `inline`，`type` 也必須設定成可以支援直接顯示的類型 e.g. `image/jpeg`, `image/png` ...
  
```csharp
// 在某一個 Controller 的 Action 中
ExtApiHelper api = new ExtApiHelper(this);
// 將會直接下載，並以亂數命名檔案名稱
return api.getStreamResponse(@"D:\a.txt");
```  
  
### `v1.17.0`
新增了 `ExtApiHelper` 擴充功能 `addIdentityPaging(ref string sql, string orderby = "since desc", string idcolumn = null, string nextId = null)`  
可以使用 ID 來做為分頁基準，細節可以參閱 [Identity Paging
](http://192.168.1.136/SideProject/ZapLib/issues/7) 中的描述，使用範例如下：

* 第一頁(第一次抓取)，`nextId` 為 `null`：

```csharp
// 在某一個 Controller 的 Action 中
ExtApiHelper api = new ExtApiHelper(this);
string sql = "select * from entity where eid>10";
string nextId = null;
api.addIdentityPaging(ref sql, "eid desc", "eid", nextId);

// 此時 sql 是：select top(51) * from entity where eid>2 order by eid desc
```

* 此時請注意，API 預設抓取 n 筆資料(在 `Web.config` 中設定 `APIDataLimit` )，但 `addIdentityPaging` 會抓取 n+1 筆資料，目的是為了判斷是否還能翻下一頁，請自行刪除最後一筆資料並取得最後一筆資料的 ID，作為下一次翻頁的基準

```csharp
SQL db = new SQL();
ModelEntity[] data = db.quickQuery<ModelEntity>(sql);

if(data.Length > int.Parse(Config.get("APIDataLimit")))
{
	// 取出最後一筆的 ID
	string nextId = data[data.Length - 1].eid.ToString();
	// 刪除最後一筆資料
	Array.Resize(ref data, data.Length - 1);
}
```
  
* 第二頁(第二次抓取)，`nextId` 為 `100`：
  
```csharp
string sql = "select * from entity where eid>10";
api.addIdentityPaging(ref sql, "eid desc", "eid", nextId);

// 此時 sql 是：with tb as(select ROW_NUMBER() over(order by eid desc) as _seq,* from entity where eid>2) select top(51) * from tb where _seq>=(select _seq from tb where eid='100') order by eid desc
```


### `v1.16.1`
新增了 `ApiController` 擴充了 SignalR 功能的 `ApiControllerSignalR` 類別，使用範例如下：  
  
首先在 Web API 專案根目錄下新增 `Startup.cs` 並撰寫以下程式碼 (這裡的設定可以依照需求調整)
```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        var hubConfiguration = new HubConfiguration();
        hubConfiguration.EnableDetailedErrors = true;
        app.MapSignalR(hubConfiguration);
    }
}
``` 
  
接下來在根目錄下的 `Global.asax` 中加入以下程式碼 (這裡的 `TimeSpan.FromSeconds(10)` 可依照需求調整)

```csharp
GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(10);
```
  
接下來新增一個 `Hubs` 目錄，並在其中新增一個 `MsgHub.cs` 類別

```csharp
[HubName("messaging")]
public class MsgHub : Hub
{
	// write something...
}
```
  
最後，開啟欲使用 SignalR 的 Controller，將原先的 `ApiController` 改為 ZapLib 提供的 `ApiControllerSignalR`，如此一來便能使用下方展示的擴充功能
  
`___Controller.cs`  

```csharp
[RoutePrefix("api")]
public class MsgController : ApiControllerSignalR<MsgHub>
{
    [HttpPost]
    [Route("msg/send")]
    public HttpResponseMessage sendMsg([FromBody]ModelMsg msg)
    {
		// 廣播給某個對象
		Hub.Clients.Client( id ).pushMsg("...");
		// 檢查特定 ID 連線狀態
		bool isAlive = IsConnectionIdAlive("Connection ID");

		string[] connectionIds = new string[]{   };

		// 廣播給某群人
		Hub.Clients.Clients(connectionIds).receiveMsg("...");
		
		// 解析一群 ID 的連線狀態
		IList<string> Alive, Dead;
        ResolveConnectionIds(connectionIds, out Alive, out Dead);

		foreach(var id in Dead)
		{
			// 無效的連線 ID
		}
	}
}
```

> 其中 `Hub` 是 `GlobalHost.ConnectionManager.GetHubContext` 取出的 `HubContext` 可以直接使用原生的功能 
> 另外 `ResolveConnectionIds(IList<string> connectionIds, out IList<string> Alive, out IList<string> Dead)` 與 `IsConnectionIdAlive(string connectionId)` 提供的功能實作原理，可以參閱[技術說明](https://www.facebook.com/groups/1634561570101701/permalink/2516506991907150/)


### `v1.15.0`

* 可以取得 SQL 最後錯誤資訊，不一定要寫入 Log 觀察，使用範例如下：  
```csharp
SQL db = new SQL("localhost", "dbname", "account", "password");
object[] o = db.quickQuery<object>("select * from class");
string error = db.getErrorMessage(); // 輸出錯誤資訊
```
  
* 修復 `ExtApiHelper.getAttachemntResponse` 無法指定副檔名的問題，，使用範例如下：  
```csharp
ExtApiHelper api = new ExtApiHelper(this);
api.getAttachemntResponse("內容","file_name.txt"); // 可以直接指定副檔名，如果沒有指定則無附檔名
```  
  
* 新增 MyLog 可以指定儲存位置與 Log 的檔案名稱
```csharp
MyLog log = new MyLog();
log.path = "D:\\Log"; // 指定儲存路徑，預設為抓取 config 中的 Storage 設定，如果都沒有指定則不會進行 Log 寫入
log.name = "mylog"; // 指定 log 檔案名稱，預設為 yyyyMMdd
log.write("wewfewfewfewfewf"); 
log.write("safawfqafw");
```
  
* 新增 Fetch 可以取得 Response 的 Header 資訊 (如果無法取得 Header 則會回傳 NULL)
```csharp
Fetch fetch = new Fetch("http://localhost");
fetch.get(null);
string header_value = fetch.getResponseHeader("key"); // 取得指定 Header 
WebHeaderCollection all_headers = fetch.getResponseHeaders();  // 取得全部 Headers
```
  

### `v1.14.0`
  * 新增了 SQL BCP 的功能，可以快速寫入大量資料，使用範例如下：  

```csharp
// 建立 DataTable 物件
var dt = new DataTable();
dt.Columns.Add("words", typeof(string));

// 將資料塞進 DataTable 中
foreach (string word in words)
{
    var row = dt.NewRow();
    row["words"] = new_word;
    dt.Rows.Add(row);
}
// 全部一次性寫入
SQL db = new SQL();
result = db.quickBulkCopy(dt, "dbo.UD");
```

## License MIT

	Copyright (C) 2018 ZapLin
	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
