//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Linq;

//namespace AtomicCore.BlockChain.TronNet
//{
//    /// <summary>
//    /// Hex Address Array Json Converter
//    /// </summary>
//    public sealed class TronNetHexAddressArrayJsonConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(string[]);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            if (typeof(string[]).IsAssignableFrom(objectType))
//            {
//                JArray jas = JArray.Load(reader);

//                return jas.Select(s =>
//                    TronNetECKey.ConvertToTronAddressFromHexAddress(s.ToString())
//                ).ToArray();
//            }

//            throw new Exception("object type must be 'string[]'");
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            if (value is string[] tronAddressies)
//            {
//                string[] hexAddressies = tronAddressies.Select(s => TronNetECKey.ConvertToHexAddress(s)
//                ).ToArray();

//                JArray ja = JArray.FromObject(hexAddressies, serializer);
//                string json_raw = ja.ToString();

//                writer.WriteRawValue(json_raw);
//                return;
//            }

//            throw new Exception(string.Format("value type must be 'string[]'..."));
//        }
//    }
//}
