# API Reference

本區由 [DocFX](https://dotnet.github.io/docfx/) 從 ZapLib source code 中的 XML doc comments 自動產生，列出所有 public 型別、方法、屬性、事件。

## 主要命名空間

| Namespace | 內容 |
|---|---|
| [`ZapLib`](ZapLib.yml) | `Fetch`、`SQL`、`OracleSQL`、`Mailer`、`ImplicitMailer`、`RegExp`、`ExtApiHelper`、`ApiControllerSignalR<T>`、`DynamicObject`、`ClassMirror`、`MyLog`、`LogExecTime`、`ZipHelper`、`ExpParam` 等核心類別 |
| [`ZapLib.Utility`](ZapLib.Utility.yml) | `Config`、`Cast`、`Mirror`、`QueryString`、`JXPath`、`DllLoader`、`MD5` |
| [`ZapLib.Security`](ZapLib.Security.yml) | `Crypto`、`ValidPlatformAttribute`、`Const` |
| [`ZapLib.Json`](ZapLib.Json.yml) | `JsonReader`、`IJsonTuple`、`ObjectTuple`、`ArrayTuple`、`ValueTuple` |
| [`ZapLib.Model`](ZapLib.Model.yml) | `ModelFile`、`ModelLog`、`ModelMultiPart` |

## 如何使用本區

* 左側 sidebar 可瀏覽完整型別樹
* 每個型別頁列出該型別所有 public 成員、繼承關係、實作介面
* 想看「**這個 API 怎麼用、為什麼這樣設計**」請回到 [Articles](../articles/getting-started.md) — 那才是教學區

## 找不到想要的型別？

* 它可能是 `internal` 或 `private` — 本區只列公開 API
* 它可能尚未補 XML doc comment — 仍會出現在型別樹但說明欄空白
* 它可能是 ZapLib 依賴的第三方型別（`HttpClient`、`SqlConnection` 等）— 請查 .NET 官方文件
