# Mailer

`Mailer` 是 ZapLib 對 SMTP 寄信的封裝，底層採用 [MailKit](https://github.com/jstedfast/MailKit) — 比 .NET 內建的 `System.Net.Mail` 更穩定、支援更多現代 SMTP server 功能。

## Namespace

```csharp
using ZapLib;
```

## Send a Simple Mail

```csharp
Mailer mailer = new Mailer(
    MAIL_HOST: "smtp.gmail.com",
    MAIL_ACT:  "sender@gmail.com",
    MAIL_PWD:  "your-app-password",
    MAIL_PORT: 587,
    MAIL_SSL:  true
);

bool ok = mailer.Send(
    to:      "alice@example.com",
    subject: "Hello from ZapLib",
    body:    "<h1>Hello!</h1><p>This is a test mail.</p>"
);

if (!ok)
{
    Console.WriteLine("寄信失敗：" + mailer.ErrMsg);
}
```

> **body 是 HTML 格式**。要寄純文字也 OK — HTML 會自然渲染純文字。

## Send to Multiple Recipients

`to`、`cc`、`bcc` 都接受用**逗號**分隔的多位收件人：

```csharp
mailer.Send(
    to:      "alice@example.com, bob@example.com",
    subject: "週報",
    body:    "<p>本週進度…</p>",
    cc:      "manager@example.com",
    bcc:     "audit@example.com"
);
```

## With Attachments

第六個參數傳檔案路徑陣列：

```csharp
mailer.Send(
    to:      "alice@example.com",
    subject: "請查收報表",
    body:    "<p>附件為本月銷售報表</p>",
    cc:      null,
    bcc:     null,
    attchments: new[]
    {
        @"C:\reports\2026-05-sales.xlsx",
        @"C:\reports\2026-05-summary.pdf"
    }
);
```

不存在的檔案路徑會被靜默忽略，**不會中斷寄信流程**。

## Inline Image with Content-ID

需要在 HTML body 中嵌入圖片？用 `AddAttachments()` 先加入附件、拿到 `content-id`、再寫進 `<img src="cid:...">`：

```csharp
Mailer mailer = new Mailer("smtp.example.com", "noreply@example.com", "pwd");

string cid = mailer.AddAttachments(@"C:\images\logo.png");

string body = $@"
    <p>歡迎使用 ZapLib！</p>
    <img src='cid:{cid}' />
";

mailer.Send(
    to:      "user@example.com",
    subject: "歡迎",
    body:    body
);
```

## Retry Mechanism

預設失敗會自動重寄 **1 次**。可由建構子第六個參數調整：

```csharp
// 完全不重試
Mailer m1 = new Mailer("host", "act", "pwd", MAIL_RETRY: 0);

// 最多重試 3 次
Mailer m2 = new Mailer("host", "act", "pwd", MAIL_RETRY: 3);
```

## Configuration

| 建構子參數 | 預設 | 說明 |
|---|---|---|
| `MAIL_HOST` | — | SMTP server 位置（必填） |
| `MAIL_ACT` | — | 登入帳號 / 寄件者位址 |
| `MAIL_PWD` | — | 登入密碼。若為空字串，將跳過 SMTP 認證（適合內部無認證的 relay） |
| `MAIL_PORT` | `587` | SMTP server 埠號 |
| `MAIL_SSL` | `true` | 是否啟用 SSL（StartTLS） |
| `MAIL_RETRY` | `1` | 失敗重試次數 |

### 屬性設定

```csharp
Mailer mailer = new Mailer("host", "act", "pwd");

// 細部調整 SSL/TLS 連線模式
mailer.SecureSocketOption = MailKit.Security.SecureSocketOptions.SslOnConnect;
```

`SecureSocketOption` 可設定為：

| 選項 | 適用 |
|---|---|
| `Auto` | 自動偵測（預設、`MAIL_SSL=false` 時） |
| `None` | 完全不加密（測試環境） |
| `SslOnConnect` | 連線就用 SSL（通常用 465 port） |
| `StartTls` | 連線後升級為 TLS（`MAIL_SSL=true` 時的預設值） |
| `StartTlsWhenAvailable` | 有 TLS 就用，沒有就明文 |

## Common SMTP Settings

### Gmail

```csharp
Mailer mailer = new Mailer(
    "smtp.gmail.com",
    "your-account@gmail.com",
    "your-app-password",     // 不是 Gmail 密碼，而是 App Password
    587,
    true
);
```

> Gmail 從 2022 年起不再支援「低安全性 App」。請至 Google 帳戶 → 兩步驟驗證 → App Passwords 產生專用密碼。

### Outlook / Office 365

```csharp
Mailer mailer = new Mailer(
    "smtp.office365.com",
    "your-account@company.com",
    "your-password",
    587,
    true
);
```

### 內部 SMTP Relay（不需認證）

```csharp
Mailer mailer = new Mailer(
    "mail-relay.internal",
    "noreply@company.com",
    "",                      // 空密碼跳過認證
    25,
    false                    // 內部 relay 通常不用 SSL
);
```

## Error Handling

`Send()` 失敗時回傳 `false`，錯誤訊息從 `ErrMsg` 取得：

```csharp
if (!mailer.Send(...))
{
    Console.WriteLine("失敗：" + mailer.ErrMsg);
}
```

常見錯誤：

| 訊息片段 | 可能原因 |
|---|---|
| `MAIL_HOST is Empty` | 建構子的 `MAIL_HOST` 為 null/空字串 |
| `SslHandshakeException` | SSL 設定錯誤 — 試試切換 `SecureSocketOption` |
| `AuthenticationException` | 帳號 / 密碼錯，或 Gmail 需要 App Password |
| `SmtpCommandException` | 寄件者或收件者格式不合法、被 server 拒絕 |
