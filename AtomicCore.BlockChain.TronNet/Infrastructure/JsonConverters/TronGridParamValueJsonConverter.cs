using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid ParamValue Json Converter
    /// </summary>
    public class TronGridParamValueJsonConverter : JsonConverter
    {
        /// <summary>
        /// interface type
        /// </summary>
        private static readonly Type s_interfaceType = typeof(ITronGridTransactionParamValue);

        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return s_interfaceType.IsAssignableFrom(objectType);
        }

        /// <summary>
        /// reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject pvo = JObject.Load(reader);

            var param_val = pvo.ToObject<TronGridTransactionParamValue>();
            param_val.SetJObject(pvo);

            return param_val;
        }

        /// <summary>
        /// writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }
    }
}
