# Security - Crypto

`Crypto` 提供 MD5、DES 加解密、亂數字串等基本工具。底層採用 .NET 內建 `System.Security.Cryptography`，但 MD5 部分使用 ZapLib 自行實作的版本（`ZapLib.Utility.MD5`）。

> ⚠️ DES 已被 NIST 標記為**不安全**（2005 起）。新專案應改用 AES。`Crypto.DES` 系列保留主要是為了相容 [`[ValidPlatform]`](../webapi/valid-platform.md) 機制。

## Namespace

```csharp
using ZapLib.Security;
```

## MD5 Hashing

```csharp
Crypto crypto = new Crypto();
string hash = crypto.Md5("Hello, ZapLib");
Console.WriteLine(hash);
```

**輸出：**

```
N1KFvCxBoBgKxvw4j7w2bg==
```

> **回傳格式**：Base64 string（不是常見的 hex string）。如果你需要 hex 格式，請用 `Convert.ToHexString()`（.NET 5+）或自行轉換。

### Encoding 選擇

預設用 ASCII 編碼輸入字串。需要 UTF-8（含中文等多位元字元）：

```csharp
Crypto crypto = new Crypto(Encoding.UTF8);
string hash = crypto.Md5("你好，ZapLib");
```

## DES Encryption / Decryption

### Encrypt

```csharp
Crypto crypto = new Crypto();
string encrypted = crypto.DESEncryption("secret message");
string iv = crypto.IV;   // 自動產生的 IV，解密時需要

Console.WriteLine($"Encrypted: {encrypted}");
Console.WriteLine($"IV: {iv}");
```

### Decrypt

```csharp
Crypto crypto = new Crypto();
string original = crypto.DESDecryption(encrypted, iv);
Console.WriteLine(original);   // "secret message"
```

### 重用同一個 IV

```csharp
Crypto crypto = new Crypto();
crypto.IV = "fixedIV8";   // 固定 8 字元

string a = crypto.DESEncryption("text 1");
string b = crypto.DESEncryption("text 2");

// 解密時用同一個 IV
string da = crypto.DESDecryption(a, "fixedIV8");
string db = crypto.DESDecryption(b, "fixedIV8");
```

> ⚠️ **重用 IV 會降低加密強度**。生產環境每次加密都應該用新 IV，並把 IV 附在密文一起傳輸。

## Random String

產生 `[A-Za-z0-9]` 範圍內的隨機字串：

```csharp
Crypto crypto = new Crypto();
string token = crypto.RandomString(32);
// 輸出範例：aB3xK9pMz7tQwR2sLv8nY4hG1jF6dC0e
```

> ⚠️ **不是密碼學等級的亂數**。內部用 `new Random()`，**可預測、可被攻擊**。
>
> **不適合**：API token、session id、密碼重設碼。
> **適合**：測試資料、UI placeholder、display-only ID。
>
> 需要密碼學亂數請用：
>
> ```csharp
> using System.Security.Cryptography;
> byte[] bytes = new byte[32];
> RandomNumberGenerator.Fill(bytes);
> string secureToken = Convert.ToBase64String(bytes);
> ```

## Security Notes

### Const.Key — 內建金鑰

`Crypto.DESEncryption` 使用的金鑰是寫死在 `ZapLib.Security.Const.Key` 的字串：

```csharp
namespace ZapLib.Security
{
    public static class Const
    {
        public static readonly string Key = "...";       // DES 金鑰
        public static readonly string GodKey = "...";    // ValidPlatform bypass key
    }
}
```

⚠️ **這是這個機制的根本弱點**：

* 反編譯 ZapLib.dll 就能拿到金鑰
* 所有用 ZapLib 的專案共用同一支金鑰
* 沒有金鑰輪替機制

**生產環境建議**：

1. Fork ZapLib 並改 `Const.Key`，編成自家私有 NuGet
2. 或改造 `Crypto` 從 `Config` 讀取金鑰，搭配 KMS
3. 或完全捨棄 `Crypto`，改用 AES + HMAC

## When to Use What

| 需求 | ZapLib | 替代方案 |
|---|---|---|
| 簡單訊息簽章（內部信任） | `Crypto.Md5` | 同上 |
| 內部 S2S API 驗證 | `Crypto.DESEncryption` + `[ValidPlatform]` | JWT / OAuth |
| 密碼儲存 | ❌ 不要用 MD5 / DES | `BCrypt.Net-Next` / `Microsoft.AspNetCore.Identity` |
| 對外 API 加密通訊 | ❌ 不要用 | HTTPS + AES-GCM |
| 密碼學等級亂數 | ❌ 不要用 `RandomString` | `RandomNumberGenerator` |

## See Also

* [MD5 (Custom Implementation)](md5.md) — `Crypto.Md5` 背後使用的版本
* [`[ValidPlatform]`](../webapi/valid-platform.md) — `Crypto` 的主要使用者
