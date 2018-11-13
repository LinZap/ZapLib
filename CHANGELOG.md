# ChangeLog

�睊����

## `v1.21.0`

### Fetch API �s�W�\��

**Form Data**

Fetch �w�]�|�H json �s�X�ǰe��ơA�p�G�ݭn�ǲΪ� From �s�X�榡�A�i�H�b `contentType` �����w

```csharp
Fetch f = new Fetch("https://httpbin.org/post");

// �w�]
f.contentType = "application/json";

// �ϥ� URL �s�X�ǰe��� x-www-form-urlencoded
f.contentType = "application/x-www-form-urlencoded";

// �ϥζǲ� Form �ǰe���
f.contentType = "multipart/form-data"
```
  
**Fetch �{�b�i�H�ǰe `Dictionary` ����**


**�ܽd�G**

```csharp
Dictionary<string, string> data = new Dictionary<string, string>();
data.Add("test", "123");
```
> �u��O `Dictionary<string, string>` ���i����L���A


### SQL API �s�W�\��

## `v1.20.0`

### SQL API �s�W�\��


**�ϥγs�u�r��s�u**

SQL API �i�H�ϥΦۭq�� Connection String �i��s�u  

```csharp
SQL db = new SQL("Data Source=(LocalDb)\MSSQLLocalDB;Initial ..."); // �ϥΦۭq Connection String �s�u
```

**`getConnection()`** 

�åB�s�W�F `getConnection()` ��k���N����u��� `connet()` ��k���o�ثe�s�u���󪺤覡  

```csharp
SQL db = new SQL(); 
db.connect();
SqlConnection myConn = db.getConnection();
```


**`quickDynamicQuery()` �P `dynamicFetch()`**

�s�W�F�ʺA�ֳt�d�� API�A�N��^ `dynamic[]` ���A����ơA�d�ߥ��Ѥ@�˦^�� `null`  

```csharp
SQL db = new SQL();
dynamic[] data = db.quickDynamicQuery("select * from entity");

for(int i = 0; i < data.Length; i++)
{
    Trace.WriteLine((string)data[i].cname);
}
```

�p�G�ݭn��ʳs�u�A�i��Ӹ`�ާ@

```csharp
SQL db = new SQL(); 
db.connet();
SqlDataReader stmt = db.query(sql);
dynamic[] data = dynamicFetch(stmt);
stmt.Close();
db.close();
```

**`quickDynamicQuery()` �P `dynamicExec()`**

�s�W�F�ֳt���� stored procedure�A�N��^ `dynamic` ���A����ơA�d�ߥ��Ѥ@�˦^�� `null`  

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

�p�G�ݭn��ʳs�u�A�i��Ӹ`�ާ@

```csharp
SQL db = new SQL();
db.connet();
dynamic obj = dynamicExec(sql, input_para, output_para);
db.close();
```



## `v1.19.1`

�s�W�F���s��**���x����**�\��� `using ZapLib.Security`  

**Controller �۰�����**

���� WebApi Controller �s������ `[ValidPlatform]` �Ω����ҬO�_���H�����ШD�A�ϥΤ覡�p�U�G

```csharp
// �b�Y�@�� Controller �� Action ��
[ValidPlatform]
[Route("test")]
public HttpResponseMessage test_file()
{
    // �p�G���ҳq�L�~�|�i�J�o�� Action    
}
```  
    
**`Fetch` ���[����**  

�p�G�ϥ� `Fetch` �I�s�㦳 `[ValidPlatform]` ���Ҫ� API�A�i�H�ҥ� `validPlatform` �אּ `true` �K�|�۰ʪ��[���Ҫ���T  

```csharp
Fetch2 f = new Fetch2("https://httpbin.org/post");
// ���[���x���Ҹ�T
f.validPlatform = true;
object res = f.post<object>(new { data = "123", result = true, number = 123 });
``` 

**`�t�Ϊ��_` �P `�W���_��`**

�n�q�L���x���ҡA��ӥ��x�����n���ۦP�� **`�t�Ϊ��_`**�A�p�G�n���ܹw�]�� **`�t�Ϊ��_`**�A�i�H�b�t�ζi�J�I�K�W�]�w

`WebApiConfig.cs`  
```csharp
ZapLib.Security.Const.Key = "�s�����_";
``` 

���F�}�o�H����K�A���үd���@�ӫ�� **`�W���_��`** �i�H�����q�L���ҡA�w�]�� `nvOcQMfERrASHCIuE797`�A�]�i�H�b�t�ζi�J�I�ק復

`WebApiConfig.cs`  
```csharp
ZapLib.Security.Const.GodKey = "�W���_��";
``` 


**���Ҥ覡�P�W��**

���x���ҹ�@�F [DES](https://zh.wikipedia.org/wiki/%E8%B3%87%E6%96%99%E5%8A%A0%E5%AF%86%E6%A8%99%E6%BA%96) ��k�A���O�y�@��}�A�ШD�ݷ|�A HTTP Header ���[ 3 �������  

| key  |  description |
| --------  | -------- | 
| `Channel-Signature` | �N�ǰe����ƥH MD5 �[�K�᪺�r��A�@���ШD��ñ�� |  
| `Channel-Iv` | �@�ըC�����|�üƲ��ͪ��[�K�ؤl |  
| `Channel-Authorization` | �H `Channel-Signature` + `Channel-Iv` + `�t�Ϊ��_` �ϥ� DES ����᪺���G |

�p�G�b Controller ���[�F `[ValidPlatform]` �h�|�b�i�J Action ���e�A���N `Channel-Signature` + `Channel-Iv` + `�t�Ϊ��_` �b�i��@�� DES ����A��� `Channel-Authorization` �ƭȬO�_�ۦP�A�p�G�ۦP�N�|���ҳq�L�C   
���Ҥ��q�L�ɡA�|�����^�� `401 Unauthorized` �����v�^���C




## `v1.18.2`

�s�W�F `ExtApiHelper.getStreamResponse` �αq���A��������ɮרåH��y�覡�^���A�ϥνd�Ҧp�U�G
   
**`getStreamResponse`**  
  
| arg  |  type | required | description |
| --------  | -------- | -------- |  -------- |
| file         |   string �� byte[]   | Y | �ɮ׹�����| �� �s���O���餤���ɮ� |
| name         |   string     | N | �ɮצW�١A�p�G�S�������A�h�|�H�üƩR�W |
| type         |   string     | N | �ɮ� MIME TYPE�A�p�G�S�������A�h�|�H�w�] `application/octet-stream` |
| disposition  |   string     | N | ��y�覡�A�w�]�� `attachment` �|�����U���ɮסA�p�G�n�b�s��������ܡA�Ҧp�Ϥ��A�i�H�]�w�� `inline` |
  
> �`�N�G`disposition` �p�G�]�m�� `inline`�A`type` �]�����]�w���i�H�䴩������ܪ����� e.g. `image/jpeg`, `image/png` ...
  
```csharp
// �b�Y�@�� Controller �� Action ��
ExtApiHelper api = new ExtApiHelper(this);
// �N�|�����U���A�åH�üƩR�W�ɮצW��
return api.getStreamResponse(@"D:\a.txt");
```  
  
## `v1.17.0`
�s�W�F `ExtApiHelper` �X�R�\�� `addIdentityPaging(ref string sql, string orderby = "since desc", string idcolumn = null, string nextId = null)`  
�i�H�ϥ� ID �Ӱ���������ǡA�Ӹ`�i�H�Ѿ\ [Identity Paging
](http://192.168.1.136/SideProject/ZapLib/issues/7) �����y�z�A�ϥνd�Ҧp�U�G

* �Ĥ@��(�Ĥ@�����)�A`nextId` �� `null`�G

```csharp
// �b�Y�@�� Controller �� Action ��
ExtApiHelper api = new ExtApiHelper(this);
string sql = "select * from entity where eid>10";
string nextId = null;
api.addIdentityPaging(ref sql, "eid desc", "eid", nextId);

// ���� sql �O�Gselect top(51) * from entity where eid>2 order by eid desc
```

* ���ɽЪ`�N�AAPI �w�]��� n �����(�b `Web.config` ���]�w `APIDataLimit` )�A�� `addIdentityPaging` �|��� n+1 ����ơA�ت��O���F�P�_�O�_�ٯ�½�U�@���A�Цۦ�R���̫�@����ƨè��o�̫�@����ƪ� ID�A�@���U�@��½�������

```csharp
SQL db = new SQL();
ModelEntity[] data = db.quickQuery<ModelEntity>(sql);

if(data.Length > int.Parse(Config.get("APIDataLimit")))
{
	// ���X�̫�@���� ID
	string nextId = data[data.Length - 1].eid.ToString();
	// �R���̫�@�����
	Array.Resize(ref data, data.Length - 1);
}
```
  
* �ĤG��(�ĤG�����)�A`nextId` �� `100`�G
  
```csharp
string sql = "select * from entity where eid>10";
api.addIdentityPaging(ref sql, "eid desc", "eid", nextId);

// ���� sql �O�Gwith tb as(select ROW_NUMBER() over(order by eid desc) as _seq,* from entity where eid>2) select top(51) * from tb where _seq>=(select _seq from tb where eid='100') order by eid desc
```


## `v1.16.1`
�s�W�F `ApiController` �X�R�F SignalR �\�઺ `ApiControllerSignalR` ���O�A�ϥνd�Ҧp�U�G  
  
�����b Web API �M�׮ڥؿ��U�s�W `Startup.cs` �ü��g�H�U�{���X (�o�̪��]�w�i�H�̷ӻݨD�վ�)
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
  
���U�Ӧb�ڥؿ��U�� `Global.asax` ���[�J�H�U�{���X (�o�̪� `TimeSpan.FromSeconds(10)` �i�̷ӻݨD�վ�)

```csharp
GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(10);
```
  
���U�ӷs�W�@�� `Hubs` �ؿ��A�æb�䤤�s�W�@�� `MsgHub.cs` ���O

```csharp
[HubName("messaging")]
public class MsgHub : Hub
{
	// write something...
}
```
  
�̫�A�}�ұ��ϥ� SignalR �� Controller�A�N����� `ApiController` �אּ ZapLib ���Ѫ� `ApiControllerSignalR`�A�p���@�ӫK��ϥΤU��i�ܪ��X�R�\��
  
`___Controller.cs`  

```csharp
[RoutePrefix("api")]
public class MsgController : ApiControllerSignalR<MsgHub>
{
    [HttpPost]
    [Route("msg/send")]
    public HttpResponseMessage sendMsg([FromBody]ModelMsg msg)
    {
		// �s�����Y�ӹ�H
		Hub.Clients.Client( id ).pushMsg("...");
		// �ˬd�S�w ID �s�u���A
		bool isAlive = IsConnectionIdAlive("Connection ID");

		string[] connectionIds = new string[]{   };

		// �s�����Y�s�H
		Hub.Clients.Clients(connectionIds).receiveMsg("...");
		
		// �ѪR�@�s ID ���s�u���A
		IList<string> Alive, Dead;
        ResolveConnectionIds(connectionIds, out Alive, out Dead);

		foreach(var id in Dead)
		{
			// �L�Ī��s�u ID
		}
	}
}
```

> �䤤 `Hub` �O `GlobalHost.ConnectionManager.GetHubContext` ���X�� `HubContext` �i�H�����ϥέ�ͪ��\�� 
> �t�~ `ResolveConnectionIds(IList<string> connectionIds, out IList<string> Alive, out IList<string> Dead)` �P `IsConnectionIdAlive(string connectionId)` ���Ѫ��\���@��z�A�i�H�Ѿ\[�޳N����](https://www.facebook.com/groups/1634561570101701/permalink/2516506991907150/)


## `v1.15.0`

* �i�H���o SQL �̫���~��T�A���@�w�n�g�J Log �[��A�ϥνd�Ҧp�U�G  
```csharp
SQL db = new SQL("localhost", "dbname", "account", "password");
object[] o = db.quickQuery<object>("select * from class");
string error = db.getErrorMessage(); // ��X���~��T
```
  
* �״_ `ExtApiHelper.getAttachemntResponse` �L�k���w���ɦW�����D�A�A�ϥνd�Ҧp�U�G  
```csharp
ExtApiHelper api = new ExtApiHelper(this);
api.getAttachemntResponse("���e","file_name.txt"); // �i�H�������w���ɦW�A�p�G�S�����w�h�L���ɦW
```  
  
* �s�W MyLog �i�H���w�x�s��m�P Log ���ɮצW��
```csharp
MyLog log = new MyLog();
log.path = "D:\\Log"; // ���w�x�s���|�A�w�]����� config ���� Storage �]�w�A�p�G���S�����w�h���|�i�� Log �g�J
log.name = "mylog"; // ���w log �ɮצW�١A�w�]�� yyyyMMdd
log.write("wewfewfewfewfewf"); 
log.write("safawfqafw");
```
  
* �s�W Fetch �i�H���o Response �� Header ��T (�p�G�L�k���o Header �h�|�^�� NULL)
```csharp
Fetch fetch = new Fetch("http://localhost");
fetch.get(null);
string header_value = fetch.getResponseHeader("key"); // ���o���w Header 
WebHeaderCollection all_headers = fetch.getResponseHeaders();  // ���o���� Headers
```
  

## `v1.14.0`
  * �s�W�F SQL BCP ���\��A�i�H�ֳt�g�J�j�q��ơA�ϥνd�Ҧp�U�G  

```csharp
// �إ� DataTable ����
var dt = new DataTable();
dt.Columns.Add("words", typeof(string));

// �N��ƶ�i DataTable ��
foreach (string word in words)
{
    var row = dt.NewRow();
    row["words"] = new_word;
    dt.Rows.Add(row);
}
// �����@���ʼg�J
SQL db = new SQL();
result = db.quickBulkCopy(dt, "dbo.UD");
```