using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet ContractType Json Converter
    /// </summary>
    public sealed class TronNetContractTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TronNetContractType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            if (reader.Value is string enumName)
            {
                bool flag = Enum.TryParse(enumName, true, out TronNetContractType enumType);
                if (!flag)
                    return TronNetContractType.UnKnow;

                return enumType;
            }
            else if (reader.Value is int enum_val)
                return (TronNetContractType)enum_val;
            else
                return TronNetContractType.UnKnow;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToInt32(value));
        }
    }
}
