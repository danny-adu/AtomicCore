using Newtonsoft.Json;
using System;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc tx receipt status converter
    /// </summary>
    public sealed class BscTxReceiptStatusConverter : JsonConverter
    {
        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType) => true;

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

            if (!(reader.Value is string hex))
                return BscReceiptStatus.Failure;

            if (hex.Equals("0x1", StringComparison.OrdinalIgnoreCase))
                return BscReceiptStatus.Success;

            return BscReceiptStatus.Failure;
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is BscReceiptStatus status)
                writer.WriteValue(status == BscReceiptStatus.Success ? "0x1" : "0x0");

            throw new TypeAccessException(nameof(value));
        }
    }
}
