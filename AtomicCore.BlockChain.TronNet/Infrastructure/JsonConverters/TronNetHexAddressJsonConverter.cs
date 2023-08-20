//using Newtonsoft.Json;
//using System;

//namespace AtomicCore.BlockChain.TronNet
//{
//    /// <summary>
//    /// Hex Address Json Converter
//    /// </summary>
//    public sealed class TronNetHexAddressJsonConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(string);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.Value == null)
//                return null;

//            string hexAddress = reader.Value.ToString();

//            return TronNetECKey.ConvertToTronAddressFromHexAddress(hexAddress);
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            writer.WriteValue(TronNetECKey.ConvertToHexAddress(value.ToString()));
//        }
//    }
//}
