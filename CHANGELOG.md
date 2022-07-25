# ChangeLog

改版紀錄


## `2.2.0`

1. 寄信功能 Mailer 改為使用 [MailKit](https://github.com/jstedfast/MailKit) 支援 TLS1.2 更安全的加密協議 (介面與之前完全相同，可向下相容)

> 備註：[原先 SmtpClient 不支援 TLS 且被微軟棄用](https://developpaper.com/system-net-mail-smtpclient-failed-to-send-email-through-ssl-tls-protocol/)

## `2.1.2`


1. 可以設定 Config 參數 `MyLog`，來指定 Log 寫入的位置 (如果沒有設置則會採用 `Storage`)
2. 新增全新 Config 參數 `LogExecTime`，當設為 `true`時，Log 將會自動紀錄 SQL 的執行時間
3. 新增全新類別 `LogExecTime`，可以記錄程式的執行時間 (Config 參數 `LogExecTime` 設為 `true` 時才會生效)

```csharp
LogExecTime lxt = new LogExecTime("Unit Test");
Thread.Sleep(1500);
lxt.Log();
```

**Log**

```
[11:40:19] [Log Exec Time] Unit Test
Takes 1.514 second
```


## `2.1.1`

1. 修復 ExtApiHelper 中用到 Stream 物件的程式，資源不正常釋放的問題

## `2.1.0`

1. 將 ExtApiHelper 的 Response 與 Request 改為 public 存取
2. 新增 ZipHelper 快速 Zip 壓縮的類別函式庫 
3. 新增 MyLog.Read(page) 方法，能以翻頁的方式讀取 Log 檔案
4. 新增 Fetch.Patch 方法
5. 修改 MyLog 建構子，以利向下相容
6. 允許設置 ExtApiHelper 的 Encoding 修改後，所有 Response 都可以套用
7. 修改所有 ExtApiHelper 中用到 Stream 物件的程式，全部加上 using 釋放資源

#### 更新套件

* jQuery  1.6.4 → 3.6.0
* Microsoft.AspNet.SignalR 2.2.3 → 2.4.3
* Microsoft.AspNet.SignalR.Core 2.2.3 → 2.4.3
* Microsoft.AspNet.SignalR.JS 2.2.3 → 2.4.3
* Microsoft.AspNet.SignalR.SystemWeb 2.2.3 → 2.4.3
* Microsoft.AspNet.WebApi.Client 5.2.4 → 5.2.7
* Microsoft.AspNet.WebApi.Core 5.2.4 → 5.2.7
* Microsoft.AspNet.WebApi.WebHost 5.2.4 → 5.2.7
* Microsoft.Owin 2.1.0 → 4.2.0
* Microsoft.Owin.Host.SystemWeb 2.1.0 → 4.2.0
* Microsoft.Owin.Security 2.1.0 → 4.2.0
* Newtonsoft.Json 11.0.2 → 12.0.1

## `2.0.12`

1. 新增以 Stream 作為輸入參數的 API Response 函數 `GetStreamResponse`
2. SQL 支援類別的成員是 Nullable Enum 資料類型的 property  (資料會嘗試轉換到 Enum，如果失敗則會容錯成 Enum 的預設值，不會變成 null)
3. `Cast.To` 如果嘗試轉換成 Enum 類別，則會自動容錯成呼叫 Enum.To 函數
4. Config 新增 `Config.Refresh` 方法，如果應用程式運行過程中可能會改變 `.config` 檔案，可以自行決定何時刷新 Config 中的快取 (cache)
5. 新增 `JXPath.GetValue` Utility 類別方法，允許指定 XPath 來取出特定 `dynamic` 或 JSON 字串中指定路徑位置的數值
6. 新增  `Mirror.GetClasses` 可以掃描全系統 class 取得特定 class 或取得有繼承 class 的 classes
7. 新增全新功能 `ClassMirror` 可以輸入指定的 Type 並建立實體
8. 新增全新 `DllLoader` Utility 類別方法，允許指定 dll 檔案路徑並建立 assembly 組件

## `2.0.11`

1. 增加 標註棄用說明 `SQL.Exec` 已標註棄用，將在下一個版本刪除
2. 修正 `ExtApiHelper` 呼叫 `GetJsonBody<T>` 無法正常取得資料的問題
3. 修正 執行 `MyLog.write` 時，如果 `Storage` 目錄不存在會造成 stackoverflow 崩潰的問題，目錄不存在時，將完全不執行
4. 修正 使用 `Fetch` 傳入不合法的 URL 時會直接崩潰的問題，並且會自動 Log 提示
5. 優化 執行 `Mirror.Assign` 或 `Cast.To` 時，如果目標 Type 為 `Nullable<T>` 將會嘗試使用 `T` 底層類別 (under type) 重新嘗試轉換

## `2.0.10`

* 修正上傳檔案參數撰寫錯誤(#32)的 bug

## `2.0.9`


1. 修正 query string 中文變亂碼的問題
2. 增加 sql order by 白名單過濾
3. 修正 ValidPlatform 少做一次內容驗證問題
4. 增加 fetch 取得 header 自動轉型
5. 修正 Mirror.Member 可能會噴錯的問題
6. 新增 DynamicObject 動態實體物件
7. 修正 ExpParam 在遇到變數名稱相似的時候 ，會取代錯誤的問題
8. 修正 SQL getErrorMessage 有時候無法取得完整的 Exception 資訊的問題

## `2.0.8`

* Fixed Fetch Oject Disposed Exception
* Fixed Proxy setting.

## `2.0.7`

* Add transaction options in SQL
* Add get headers in ExtApiHelper

## `2.0.6`

1. `ZapLib/ExtApiHelper.cs` Add `GetHeader` and `GetHeader<T>` function
2. `ZapLib/Utility/Cast.cs` Add `ToEnum` and `ToEnum<T>` function

## `2.0.5`

* Add New Feature JsonReader

## `2.0.4`

1. implement interface  `ISQLParam` can custom function to run way of your process
2. add `SQLTypeAttribute ` defined SQL output model for mapping data type
3. implement interface  `ISQLTypeAttribute` can custom function to run way of your process
4. `ZapLib/Utility/Mirror.cs` Add `GetCustomAttributes<T>`


## `2.0.3`

* New Feature  SQL 參數擴展 ，允許使用  `ExpParam<T>` 以陣列當作資料，並自動生成 Prepare statement args


## `2.0.0 ~ 2.0.2`

* beta

## `v1.22.0`


### MyLog 添加新的機制

MyLog 從這個版本開始，**將會忽略因為 Log 檔案無法寫入的錯誤** (e.g. 多執行續共寫一個 Log 檔案)，並且允許全局關閉 ZapLib 內建的 Log 訊息。

**Force Log**

如果你仍需要在無法寫入 Log 檔案時，完整保留 Log 資訊，可以在 `App.config` 或是 `Web.config` 中添加 `ForceLog`，ZapLib 將自動把資訊寫在另一個檔案中，它看起來像是這樣 **`原本 Log 檔名-0f8fad5b-d9cb-469f-a165-70867728950e.txt`**，它會有助於你進行 debug 作業

```xml
<add key="ForceLog" value="True"/>
```
  
**Silent Mode**

如果你需要全局關閉 ZapLib 內建的 Log 訊息，可以在 `App.config` 或是 `Web.config` 中添加 `SilentMode`，一旦設置了這個選項，ZapLib 將關閉所有內建的 Log 訊息，但您還是可以呼叫 `MyLog` 功能進行紀錄，不會有所影響。

```xml
<add key="SilentMode" value="True"/>
```

**更新指南**

1. 如果你需要在 Log 檔無法寫入強破紀錄訊息，可以開啟 Force Log 的功能
2. 如果你需要關閉所有 ZapLib 的內建 log，可以開啟 Silent Mode 的功能
3. 如果你沒有以上需求，可以無需做任何修改，直接更新

## `v1.21.0`

### Fetch API 新增功能

**Form Data**

Fetch 預設會以 json 編碼傳送資料，如果需要傳統的 From 編碼格式，可以在 `contentType` 中指定

```csharp
Fetch f = new Fetch("https://httpbin.org/post");

// 預設
f.contentType = "application/json";

// 使用 URL 編碼傳送資料 x-www-form-urlencoded
f.contentType = "application/x-www-form-urlencoded";

// 使用傳統 Form 傳送資料
f.contentType = "multipart/form-data"
```
  
**Fetch 現在可以傳送 `Dictionary` 物件**


**示範：**

```csharp
Dictionary<string, string> data = new Dictionary<string, string>();
data.Add("test", "123");

Fetch f = new Fetch("https://httpbin.org/post");
f.contentType = "application/x-www-form-urlencoded";
f.post(data);
```
> 只能是 `Dictionary<string, string>` 不可為其他型態


### SQL API 新增功能

## `v1.20.0`

### SQL API 新增功能


**使用連線字串連線**

SQL API 可以使用自訂的 Connection String 進行連線  

```csharp
SQL db = new SQL("Data Source=(LocalDb)\MSSQLLocalDB;Initial ..."); // 使用自訂 Connection String 連線
```

**`getConnection()`** 

並且新增了 `getConnection()` 方法取代原先只能用 `connet()` 方法取得目前連線物件的方式  

```csharp
SQL db = new SQL(); 
db.connect();
SqlConnection myConn = db.getConnection();
```


**`quickDynamicQuery()` 與 `dynamicFetch()`**

新增了動態快速查詢 API，將返回 `dynamic[]` 型態的資料，查詢失敗一樣回傳 `null`  

```csharp
SQL db = new SQL();
dynamic[] data = db.quickDynamicQuery("select * from entity");

for(int i = 0; i < data.Length; i++)
{
    Trace.WriteLine((string)data[i].cname);
}
```

如果需要手動連線，進行細節操作

```csharp
SQL db = new SQL(); 
db.connet();
SqlDataReader stmt = db.query(sql);
dynamic[] data = dynamicFetch(stmt);
stmt.Close();
db.close();
```

**`quickDynamicQuery()` 與 `dynamicExec()`**

新增了快速執行 stored procedure，將返回 `dynamic` 型態的資料，查詢失敗一樣回傳 `null`  

```csharp
SQL db = new SQL();

var input_para = new
{
    act = "admin",
    passportcode = "1234567890"
};

var output_para = new
{
    res = SqlDbType.Int
};

dynamic data = db.quickDynamicExec("xp_checklogin", input_para, output_para);
Trace.WriteLine((int)data.res);
```

如果需要手動連線，進行細節操作

```csharp
SQL db = new SQL();
db.connet();
dynamic obj = dynamicExec(sql, input_para, output_para);
db.close();
```



## `v1.19.1`

新增了全新的**平台驗證**功能於 `using ZapLib.Security`  

**Controller 自動驗證**

提供 WebApi Controller 新的標籤 `[ValidPlatform]` 用於驗證是否為信任的請求，使用方式如下：

```csharp
// 在某一個 Controller 的 Action 中
[ValidPlatform]
[Route("test")]
public HttpResponseMessage test_file()
{
    // 如果驗證通過才會進入這個 Action    
}
```  
    
**`Fetch` 附加驗證**  

如果使用 `Fetch` 呼叫具有 `[ValidPlatform]` 驗證的 API，可以啟用 `validPlatform` 改為 `true` 便會自動附加驗證的資訊  

```csharp
Fetch2 f = new Fetch2("https://httpbin.org/post");
// 附加平台驗證資訊
f.validPlatform = true;
object res = f.post<object>(new { data = "123", result = true, number = 123 });
``` 

**`系統金鑰` 與 `上帝鑰匙`**

要通過平台驗證，兩個平台必須要有相同的 **`系統金鑰`**，如果要改變預設的 **`系統金鑰`**，可以在系統進入點添增設定

`WebApiConfig.cs`  
```csharp
ZapLib.Security.Const.Key = "新的金鑰";
``` 

為了開發人員方便，驗證留有一個後門 **`上帝鑰匙`** 可以直接通過驗證，預設為 `nvOcQMfERrASHCIuE797`，也可以在系統進入點修改它

`WebApiConfig.cs`  
```csharp
ZapLib.Security.Const.GodKey = "上帝鑰匙";
``` 


**驗證方式與規格**

平台驗證實作了 [DES](https://zh.wikipedia.org/wiki/%E8%B3%87%E6%96%99%E5%8A%A0%E5%AF%86%E6%A8%99%E6%BA%96) 方法，但是稍作改良，請求端會再 HTTP Header 附加 3 個欄位資料  

| key  |  description |
| --------  | -------- | 
| `Channel-Signature` | 將傳送的資料以 MD5 加密後的字串，作為請求的簽章 |  
| `Channel-Iv` | 一組每次都會亂數產生的加密種子 |  
| `Channel-Authorization` | 以 `Channel-Signature` + `Channel-Iv` + `系統金鑰` 使用 DES 雜湊後的結果 |

如果在 Controller 附加了 `[ValidPlatform]` 則會在進入 Action 之前，先將 `Channel-Signature` + `Channel-Iv` + `系統金鑰` 在進行一次 DES 雜湊，比對 `Channel-Authorization` 數值是否相同，如果相同就會驗證通過。   
驗證不通過時，會直接回傳 `401 Unauthorized` 未授權回應。




## `v1.18.2`

新增了 `ExtApiHelper.getStreamResponse` 用從伺服器中抓取檔案並以串流方式回應，使用範例如下：
   
**`getStreamResponse`**  
  
| arg  |  type | required | description |
| --------  | -------- | -------- |  -------- |
| file         |   string 或 byte[]   | Y | 檔案實體路徑 或 存放於記憶體中的檔案 |
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
  
## `v1.17.0`
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


## `v1.16.1`
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


## `v1.15.0`

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
  

## `v1.14.0`
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