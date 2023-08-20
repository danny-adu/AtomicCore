using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid TRC20 KV Json Converter
    /// </summary>
    public sealed class TronGridTRC20KVJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, System.Numerics.BigInteger>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jas = JArray.Load(reader);
            var dics = new Dictionary<string, System.Numerics.BigInteger>();

            foreach (var item in jas.Children())
                if (item.First is JProperty jp)
                    dics.Add(jp.Name, System.Numerics.BigInteger.Parse(jp.Value.ToString()));

            return dics;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Dictionary<string, System.Numerics.BigInteger> dics)
            {
                var jas = new JArray();
                foreach (var item in dics)
                {
                    var jo = new JObject
                    {
                        new JProperty(item.Key, item.Value.ToString())
                    };

                    jas.Add(jo);
                }

                var json = jas.ToString();

                writer.WriteValue(json);
                return;
            }

            throw new NotImplementedException($"'TronGridTRC20KVJsonConverter' need value type is 'Dictionary<string, System.Numerics.BigInteger>', but current type is '{value.GetType().FullName}'");
        }
    }
}
