# [ZapLib](https://www.nuget.org/packages/ZapLib/)

Inspired by jQuery and Node.js, ZapLib is a lightweight library for C# that empowers developers to swiftly accomplish intricate tasks. Whether it's performing HTTP requests, executing SQL Server queries, extending .NET Web API functions, sending SMTP emails, working with regular expressions, and more, all can be achieved using straightforward code with ZapLib.

## [ChangeLog](https://github.com/LinZap/ZapLib/blob/master/CHANGELOG.md)

## Installation

**Package Manager (v1.23.0 stable)**

```
PM> Install-Package ZapLib -Version 1.23.0
```
  
**Package Manager (v2.4.8 stable)**

```
PM> Install-Package ZapLib -Version 2.4.8
```


## System requirement

Version Compatibility

| version | .NET Framework | 
| --- | --- | 
| ≥ `v2.1.0` | .NET Framework 4.7.2 | 
| ≥ `v1.23.0` | .NET Framework 4.7.2 | 
| ≥ `v1.12.0` | .NET Framework 4.5 | 
| ≤ `v1.10.0` | .NET Framework 4.0 | 


## SignalR Issue

Starting from version v1.16.0, SignalR is included. If you are using ZapLib in a .NET WebAPI project and you are not utilizing SignalR-related features, please add the configuration to your Web.config file.

```xml
<appSettings>
    <add key="owin:AutomaticAppStartup" value="false" />
</appSettings>
```
  
## [Documentation V2](https://zaplib.gitbook.io/zaplib2/)
## [Documentation V1](https://linzap.gitbooks.io/zaplib/content/)


## API Reference

* [Fetch API](https://linzap.gitbooks.io/zaplib/content/methods.md)
* [SQL API](https://linzap.gitbooks.io/zaplib/content/sql/sql-api.md)
* [Web API](https://linzap.gitbooks.io/zaplib/content/web-api/web-api.md)
* [SignalR Helper](https://linzap.gitbooks.io/zaplib/content//web-api/signalr-helper.md)
* [ValidPlatform Attribute](https://linzap.gitbooks.io/zaplib/content/web-api/validplatform-api.md)
* [Mailer API](https://linzap.gitbooks.io/zaplib/content/mailer/mailer-api.md)
* [RegExp API](https://linzap.gitbooks.io/zaplib/content/regular-expression/regexp-api.md)
* [QueryString API](https://linzap.gitbooks.io/zaplib/content/querystring-api/querystring-api.md)
* [Config API](https://linzap.gitbooks.io/zaplib/content/config-api/config-api.md)
* [MyLog API](https://linzap.gitbooks.io/zaplib/content/mylog-api/mylog-api.md)


## License MIT

	Copyright (C) 2024 ZapLin
	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.