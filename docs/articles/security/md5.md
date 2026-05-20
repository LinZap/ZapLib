# Security - MD5 (Custom Implementation)

ZapLib 提供一個**手寫的 MD5 實作** — `ZapLib.Utility.MD5`。為什麼不用 .NET 內建的 `System.Security.Cryptography.MD5`？沒有官方說明，可能是為了避免在 FIPS-enabled 環境拋例外，或單純是作者學習用途。

> ⚠️ **一般使用者請優先用 .NET 內建版本**。本類別僅在你需要繞過 FIPS、或不想拉 `System.Security.Cryptography` 命名空間時使用。
>
> 更高層級的便利封裝請看 [Crypto](crypto.md)。

## Namespace

```csharp
using ZapLib.Utility;
```

## Usage

`MD5` 是 `static` 類別，只暴露一個方法 `ComputeHash`：

```csharp
byte[] input = Encoding.UTF8.GetBytes("Hello, ZapLib");
byte[] hash = MD5.ComputeHash(input);

// 轉成 hex string
string hex = BitConverter.ToString(hash).Replace("-", "").ToLower();
Console.WriteLine(hex);
```

**輸出範例：**

```
e9a8b25b3e2d4f8a... (32 個 hex 字元)
```

## Convert to Base64

`Crypto.Md5` 內部就是把這個 `byte[]` 轉成 base64：

```csharp
byte[] input = Encoding.ASCII.GetBytes("Hello");
byte[] hash = MD5.ComputeHash(input);
string base64 = Convert.ToBase64String(hash);
```

## When to Use Built-in MD5 Instead

絕大多數情況請用 .NET 內建：

```csharp
using System.Security.Cryptography;

byte[] input = Encoding.UTF8.GetBytes("Hello");
using (var md5 = System.Security.Cryptography.MD5.Create())
{
    byte[] hash = md5.ComputeHash(input);
}
```

內建版本：

* **效能更好**（CryptoAPI 或硬體加速）
* **跨平台**（在 .NET Core/5+ 一致行為）
* **更多便利方法**（`HashData`、`HashDataAsync`、stream 版本）

## Security Notes

**MD5 已破**：

* 1996 起證實有碰撞攻擊
* 2008 起可在數秒內產生碰撞
* 2020 NIST 標記為**不適合任何安全用途**

**不要用 MD5 來**：

* 儲存密碼
* 驗證檔案完整性（對抗惡意攻擊）
* 數位簽章
* 任何「對抗對手」的場景

**可以用 MD5 來**：

* 純粹的 hashmap key
* 簡單的去重識別
* 對既有資料的相容性（例如和舊系統的握手）
* ZapLib 自己的 `[ValidPlatform]` 內部簽章（已是 trust-internal 場景）

需要單向 hash？用 **SHA-256** 或更新：

```csharp
using System.Security.Cryptography;
byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes("..."));
```

需要儲存密碼？用 **BCrypt** / **Argon2**：

```csharp
// 安裝 NuGet: BCrypt.Net-Next
string hash = BCrypt.Net.BCrypt.HashPassword("user-password");
bool ok = BCrypt.Net.BCrypt.Verify("user-password", hash);
```

## See Also

* [Crypto](crypto.md) — 使用本類別的高階封裝
* [Wikipedia - MD5](https://en.wikipedia.org/wiki/MD5)
