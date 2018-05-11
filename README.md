# ZapLib

ZapLib ���� jQuery, Node.js ���F�P�ҵo�A�b C# �]���Ѥ@�M�D�`�������禡�w�A�}�o�H������ֳt�����������\��C�L�ױz�ݭn Http �ШD�BSQL Server ���d�ߡB.NET Web Api ���X�R�禡�BSMTP �H�H�BRegular Expression... ���A�����²�檺�{���X�ӧ������I

## Installation

**Package Manager**

```
PM> Install-Package ZapLib -Version 1.15.0
```

## System requirement

* `v1.10.0` �H�e�������䴩 .NET Framework 4.0 �H�W
* `v1.12.0` �}�l�������Ȥ䴩 .NET Framework 4.5 �H�W

## API Reference

* Fetch�G
* SQL�G
* Extension Web Api Helper�G
* Mailer�G
* Regular Expression�G
* Query String�G
* Config�G
* Log�G
* ��Ȯר�(�@)�G
* ��Ȯר�(�G)�G


## ChangeLog
�睊����

### * `v1.15.0`

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
  
* �s�W Fetch �i�H���o Response �� Header ��T
  
�p�G�L�k���o Header �h�|�^�� NULL
  
```csharp
Fetch fetch = new Fetch("http://localhost");
fetch.get(null);
string header_value = fetch.getResponseHeader("key"); // ���o���w Header 
WebHeaderCollection all_headers = fetch.getResponseHeaders();  // ���o���� Headers
```
  

### * `v1.14.0`
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

## License MIT

	Copyright (C) 2018 ZapLin
	Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
