# Implicit Mailer

`ImplicitMailer` 是 ZapLib 對 **隱式 SSL（Implicit SSL）** SMTP 寄信的封裝，底層採用 [Aegis Implicit Mail (AIM)](https://github.com/nilnull/AIM)。

> **隱式 vs 顯式 SSL**：
> * **顯式（StartTLS）** — 連線到 port 587（明文）→ 升級為 TLS 加密 → 傳資料
> * **隱式（Implicit SSL）** — 連線到 port **465** 就直接是 SSL 通道
>
> 部分 SMTP server（含 Gmail 老設定）只支援其中一種。如果 [`Mailer`](mailer.md) 連不上，試試 `ImplicitMailer`。

## Namespace

```csharp
using ZapLib;
```

## Basic Usage

```csharp
ImplicitMailer m = new ImplicitMailer(
    MAIL_HOST: "smtp.gmail.com",
    MAIL_ACT:  "your-account@gmail.com",
    MAIL_PWD:  "your-app-password",
    MAIL_PORT: 465,
    MAIL_SSL:  true
);

bool ok = m.Send(
    to:      "alice@example.com",
    subject: "Hello via Implicit SSL",
    body:    "<h1>Hello!</h1><p>Sent through port 465 directly.</p>"
);

if (!ok)
{
    Console.WriteLine("失敗：" + m.ErrMsg);
}
```

## Send to Multiple Recipients

`to`、`cc`、`bcc` 都接受**逗號**分隔的多位收件人：

```csharp
m.Send(
    to:      "alice@example.com, bob@example.com",
    subject: "週報",
    body:    "<p>本週進度…</p>",
    cc:      "manager@example.com",
    bcc:     "audit@example.com"
);
```

## With Attachments

```csharp
m.Send(
    to:      "alice@example.com",
    subject: "請查收",
    body:    "<p>附件為本月報表</p>",
    cc:      null,
    bcc:     null,
    attchments: new[]
    {
        @"C:\reports\2026-05-sales.xlsx",
        @"C:\reports\2026-05-summary.pdf"
    }
);
```

## Inline Image with Content-ID

```csharp
ImplicitMailer m = new ImplicitMailer(...);

string cid = m.AddAttachments(@"C:\images\logo.png");

string body = $@"
    <p>歡迎使用 ZapLib！</p>
    <img src='cid:{cid}' />
";

m.Send("user@example.com", "歡迎", body);
```

## SSL Mode Override

可在 `Send()` 之前覆寫 `SecureSocketOption`：

```csharp
ImplicitMailer m = new ImplicitMailer(...);
m.SecureSocketOption = AegisImplicitMail.SslMode.Auto;   // 或 None / Ssl / Tls
```

| 選項 | 適用 |
|---|---|
| `Ssl` | 隱式 SSL（預設、`MAIL_SSL=true` 時） |
| `Auto` | 自動偵測（`MAIL_SSL=false` 時） |
| `None` | 完全不加密（測試環境） |
| `Tls` | 升級為 TLS |

## Authentication Type

預設用 `Base64`。其他選項：

```csharp
m.AuthenticationType = AegisImplicitMail.AuthenticationType.PlainText;
```

* `Base64`（預設）
* `PlainText`
* `Login`
* `CramMd5`

## ImplicitMailer vs Mailer

| 項目 | [`Mailer`](mailer.md) | `ImplicitMailer` |
|---|---|---|
| 底層套件 | [MailKit](https://github.com/jstedfast/MailKit) | [Aegis Implicit Mail](https://github.com/nilnull/AIM) |
| 預設 port | 587 | 465 |
| 加密方式 | StartTLS（連線後升級） | 隱式 SSL（直接 SSL 通道） |
| 主要適用 | 大部分現代 SMTP server | 老式 / 只開 465 port 的 server |
| 失敗時的 `Send()` 行為 | 直接回 `false`，重試 | 用非同步 callback 回報；`Send()` 即使連線中也會回 `true` |

> ⚠️ **`ImplicitMailer.Send()` 的回傳值「沒有那麼可信」**。它的底層 `MimeMailer.SendMail()` 是非同步的，`Send()` 可能在郵件實際送出失敗之前就回 `true`。**錯誤透過 `ErrMsg` 累積**，請在送出後等一下再檢查：
>
> ```csharp
> m.Send(...);
> Thread.Sleep(2000);   // 等非同步 callback 完成
> if (!string.IsNullOrEmpty(m.ErrMsg))
> {
>     Console.WriteLine("失敗：" + m.ErrMsg);
> }
> ```
>
> 這也是為什麼**多數情況下推薦先用 [`Mailer`](mailer.md)**，只在連線失敗時才換 `ImplicitMailer`。

## Error Handling

```csharp
if (!m.Send(...))
{
    // MAIL_HOST 空 / 同步階段就失敗
    Console.WriteLine("立即失敗：" + m.ErrMsg);
}

// 非同步寄信過程的錯誤
Thread.Sleep(2000);
if (!string.IsNullOrEmpty(m.ErrMsg))
{
    Console.WriteLine("延遲失敗：" + m.ErrMsg);
}
```

## See Also

* [Mailer](mailer.md) — 使用 MailKit 的 StartTLS 寄信工具（推薦先用）
