using Newtonsoft.Json;
using System.IO;

namespace ZapLib.Json
{
    /// <summary>
    /// Json 資料檢視輔助工具
    /// </summary>
    public class JsonReader
    {
        /// <summary>
        /// 提供對 JSON 文本數據的快速，非緩存，僅單向訪問的串流讀取器
        /// </summary>
        protected JsonTextReader JReader { get; set; }

        /// <summary>
        /// 將 Json 字串以串流方式，快速解析成允許索引存取的結構，如果無法解析將回傳 null
        /// </summary>
        /// <param name="json">Json 格式的字串</param>
        /// <returns>允許索引存取的資料結構</returns>
        public IJsonTuple Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            JReader = new JsonTextReader(new StringReader(json));
            try
            {
                if (JReader.Read())
                {
                    switch (JReader.TokenType)
                    {
                        case JsonToken.StartObject:
                            return BuildObject(null, null);
                        case JsonToken.StartArray:
                            return BuildArray(null);
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Date:
                        case JsonToken.Bytes:
                            return new ValueTuple(JReader.Value);
                        case JsonToken.Null:
                        case JsonToken.Undefined:
                            return new ValueTuple(null);
                        default:
                            return null;
                    }
                }
            }
            catch { }
            return null;
        }

        private IJsonTuple BuildObject(ObjectTuple curry_data, string key)
        {
            ObjectTuple data = curry_data ?? new ObjectTuple();
            JReader.Read();
            switch (JReader.TokenType)
            {
                case JsonToken.PropertyName:
                    key = JReader.Value.ToString();
                    data[key] = null;
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    data[key] = new ValueTuple(JReader.Value);
                    key = null;
                    break;
                case JsonToken.Null:
                case JsonToken.Undefined:
                    data[key] = new ValueTuple(null);
                    key = null;
                    break;
                case JsonToken.StartObject:
                    data[key] = BuildObject(null, null);
                    break;
                case JsonToken.StartArray:
                    data[key] = BuildArray(null);
                    break;
                case JsonToken.StartConstructor:
                    SkipConstratorToken();
                    break;
                case JsonToken.Comment:
                case JsonToken.Raw:
                case JsonToken.EndArray:
                    JReader.Read();
                    key = null;
                    break;
                default:
                    return data;
            }
            BuildObject(data, key);
            return data;
        }

        private IJsonTuple BuildArray(ArrayTuple curry_data)
        {
            ArrayTuple data = curry_data ?? new ArrayTuple();
            JReader.Read();
            switch (JReader.TokenType)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    data.Value().Add(new ValueTuple(JReader.Value));
                    break;
                case JsonToken.Null:
                case JsonToken.Undefined:
                    data.Value().Add(new ValueTuple(null));
                    break;
                case JsonToken.StartObject:
                    data.Value().Add(BuildObject(null, null));
                    break;
                case JsonToken.StartArray:
                    data.Value().Add(BuildArray(null));
                    break;
                case JsonToken.StartConstructor:
                    SkipConstratorToken();
                    break;
                case JsonToken.Comment:
                case JsonToken.Raw:
                case JsonToken.EndObject:
                case JsonToken.PropertyName:
                    JReader.Read();
                    break;
                default:
                    return data;
            }
            BuildArray(data);
            return data;
        }

        private void SkipConstratorToken()
        {
            while (JReader.Read())
                if (JReader.TokenType == JsonToken.EndConstructor) break;
        }

    }
}
