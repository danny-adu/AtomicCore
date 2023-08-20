using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Meta Link Json Converter
    /// </summary>
    public sealed class TronGridMetaLinkJsonConverter : JsonConverter
    {
        /// <summary>
        /// next
        /// </summary>
        private const string c_next = "next";

        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
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
            var jo = JObject.Load(reader);
            if (null == jo)
                return null;

            var dics = jo.ToObject<Dictionary<string, string>>();
            if (null == dics || dics.Count <= 0)
                return null;

            return new TronGridMetaLinkInfo()
            {
                Next = dics.ContainsKey(c_next) ? dics[c_next] : string.Empty
            };
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TronGridMetaLinkInfo metaLinkInfo)
            {
                string link_json = JsonConvert.SerializeObject(new Dictionary<string, string>()
                {
                    { c_next, metaLinkInfo.Next }
                });
                writer.WriteValue(link_json);
                return;
            }

            throw new NotImplementedException($"'TronGridMetaLinkJsonConverter' need value type is 'TronGridMetaLinkInfo', but current type is '{value.GetType().FullName}'");
        }
    }
}
