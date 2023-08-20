using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transaction Recipt Status JsonConverter
    /// </summary>
    public sealed class TronNetTransactionReciptStatusJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TronNetTransactionReciptStatus);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            if (reader.Value is string enumName)
                if ("SUCCESS".Equals(enumName, StringComparison.OrdinalIgnoreCase))
                    return TronNetTransactionReciptStatus.SUCCESS;

            return TronNetTransactionReciptStatus.FAILURE;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToUpper());
        }
    }
}
