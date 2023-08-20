using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Trx Unit Json Converter
    /// </summary>
    public class TronNetTrxUnitJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            bool flag = long.TryParse(reader.Value.ToString(), out long amount);
            if (!flag)
                return decimal.Zero;

            return amount / 1000000M;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is decimal amount)
                writer.WriteValue((long)amount * 1000000);
            else
                writer.WriteValue(value);
        }
    }
}
