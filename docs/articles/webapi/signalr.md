# Web API - SignalR

`ApiControllerSignalR<T>` 是一個抽象基底類別，繼承它的 `ApiController` 會自動拿到一個 SignalR Hub 的 `IHubContext` — 讓 Web API 可以直接從 server 端推訊息給已連線的 client。

> ⚙️ 從 ZapLib `v1.16.0` 起內建 SignalR 套件。如果你的 Web API 專案**不使用** SignalR，請參考 [首頁的 .NET WebAPI 注意事項](../../index.md#net-webapi-注意事項) 關閉 OWIN 自動啟動。

## Namespace

```csharp
using ZapLib;
using Microsoft.AspNet.SignalR;
```

## Define a Hub

先定義一個 SignalR Hub：

```csharp
public class ChatHub : Hub
{
    public override Task OnConnected()
    {
        Console.WriteLine($"Client {Context.ConnectionId} connected");
        return base.OnConnected();
    }
}
```

並在 `Startup.cs` 註冊：

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.MapSignalR();
    }
}
```

## Use ApiControllerSignalR<T>

讓 `ApiController` 繼承 `ApiControllerSignalR<ChatHub>`：

```csharp
public class ChatController : ApiControllerSignalR<ChatHub>
{
    [HttpPost]
    public HttpResponseMessage Broadcast(string message)
    {
        // 推播給所有連線中的 client
        Hub.Clients.All.receiveMessage(new
        {
            text = message,
            sentAt = DateTime.UtcNow
        });

        var api = new ExtApiHelper(this);
        return api.GetResponse(new { ok = true });
    }
}
```

`Hub` 屬性即 `IHubContext<ChatHub>`，可以呼叫：

* `Hub.Clients.All` — 全體
* `Hub.Clients.Client(connectionId)` — 指定一個 client
* `Hub.Clients.Group(groupName)` — 指定群組
* `Hub.Clients.AllExcept(...)` — 排除某些

## Check Connection Alive

判斷某個 `connectionId` 是否仍在線：

```csharp
public class ChatController : ApiControllerSignalR<ChatHub>
{
    [HttpGet]
    public HttpResponseMessage IsOnline(string connectionId)
    {
        bool alive = IsConnectionIdAlive(connectionId);
        var api = new ExtApiHelper(this);
        return api.GetResponse(new { connectionId, alive });
    }
}
```

## Filter Alive / Dead Connections

對一批 `connectionId` 一次性分組：

```csharp
[HttpPost]
public HttpResponseMessage SendToBatch([FromBody] string[] ids)
{
    ResolveConnectionIds(ids, out IList<string> alive, out IList<string> dead);

    // 只對在線的人推送
    foreach (var id in alive)
    {
        Hub.Clients.Client(id).notify(new { ... });
    }

    // 清理已斷線的 (例如從 DB 移除)
    foreach (var id in dead)
    {
        CleanupDeadConnection(id);
    }

    var api = new ExtApiHelper(this);
    return api.GetResponse(new
    {
        sent = alive.Count,
        dead = dead.Count
    });
}
```

## Pattern: One-to-One Notification

實務常見場景 — 「使用者 A 收到新訊息，推播給 A 已連線的所有 client」：

```csharp
public class NotificationController : ApiControllerSignalR<ChatHub>
{
    [HttpPost]
    public HttpResponseMessage Notify(int userId, string text)
    {
        // 假設 DB 記錄了 user 的 active connection ids
        var db = new SQL("DefaultConn");
        string[] connIds = db.QuickQuery<string>(
            "SELECT connection_id FROM UserConnections WHERE user_id = @uid",
            new { uid = userId }
        );

        ResolveConnectionIds(connIds, out var alive, out var dead);

        foreach (var id in alive)
        {
            Hub.Clients.Client(id).onMessage(new { text, at = DateTime.UtcNow });
        }

        // 清掉斷線的
        if (dead.Count > 0)
        {
            db.QuickQuery<dynamic>(
                "DELETE FROM UserConnections WHERE connection_id IN (...)",
                ...
            );
        }

        var api = new ExtApiHelper(this);
        return api.GetResponse(new { delivered = alive.Count });
    }
}
```

## Notes

* `[NonAction]` 已加在 `IsConnectionIdAlive` 與 `ResolveConnectionIds` — 不會被當成 API endpoint 對外暴露
* `Hub` 屬性使用 `Lazy<IHubContext>` — 第一次存取才會建立
* 同一個 controller 只能綁定**一個** Hub 類別。需要多個請各自繼承不同的 `ApiControllerSignalR<T>`

## See Also

* [ExtApiHelper](extapihelper.md)
* [ASP.NET SignalR 官方文件](https://learn.microsoft.com/aspnet/signalr/)
