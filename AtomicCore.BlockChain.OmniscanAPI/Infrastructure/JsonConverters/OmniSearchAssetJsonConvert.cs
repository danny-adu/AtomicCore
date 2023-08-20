using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search asset json convert
    /// </summary>
    public class OmniSearchAssetJsonConvert : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //bool isNullable = this.IsNullableType(objectType);
            //Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray jarr = JArray.Load(reader);

                List<OmniSearchAssetJson> list = new List<OmniSearchAssetJson>();

                OmniSearchAssetJson model = null;
                foreach (JToken item in jarr.AsJEnumerable())
                {
                    object[] objs = item.ToObject<object[]>();
                    if (objs.Length < 3)
                        continue;

                    model = new OmniSearchAssetJson();
                    model.PropertyId = Convert.ToUInt64(objs[0]);
                    model.PropertyName = objs[1].ToString();
                    model.ReferenceAddress = objs[2].ToString();

                    list.Add(model);
                }

                return list.ToArray();
            }

            throw new Exception(string.Format("Unexpected token {0} when parsing enum,Value is {1}", reader.TokenType, reader.ToString()));
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value);
        }

        /// <summary>
        /// IsNullableType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsNullableType(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            if (t.IsGenericType)
                return (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
            else
                return t.BaseType.FullName == "System.ValueType";
        }
    }
}
