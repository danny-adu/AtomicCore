using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet 
    /// </summary>
    public sealed class TronNetResourceTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TronNetResourceType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            if (reader.Value is string enumName)
            {
                bool flag = Enum.TryParse(enumName, true, out TronNetResourceType enumType);
                if (!flag)
                    return TronNetResourceType.UnKnow;

                return enumType;
            }
            else if (reader.Value is int enum_val)
                return (TronNetResourceType)enum_val;
            else
                return TronNetResourceType.UnKnow;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToInt32(value));
        }
    }
}
