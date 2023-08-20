using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Json Value to TronscanJsonStatus
    /// </summary>
    public sealed class BizTronJsonStatusConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TronscanJsonStatus);
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
            if (reader.Value == null)
                return null;

            if (reader.Value.ToString().Equals("0", StringComparison.OrdinalIgnoreCase))
                return TronscanJsonStatus.Success;
            else
                return TronscanJsonStatus.Failure;
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((int)value).ToString());
        }
    }
}
