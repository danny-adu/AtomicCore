using System;
using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// UtcTimeToMillisecondsConvert
    /// </summary>
    public class UtcTimeToMillisecondsConvert : JsonConverter
    {
        /// <summary>
        /// UTC's Time of 1970-01-01 => Local Time
        /// </summary>
        private static readonly DateTime utc_1970 = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

        /// <summary>
        /// CanConvert
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
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
            //bool isNullable = this.IsNullableType(objectType);
            //Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!IsNullableType(objectType))
                    throw new Exception(string.Format("can not null value convert to {0}.", objectType));

                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                if (!DateTime.TryParse(reader.Value.ToString(), out DateTime dt))
                    throw new Exception(string.Format("Error converting value {0} to type '{1}'", reader.Value, objectType));

                return (long)dt.Subtract(utc_1970).TotalSeconds;
            }
            if (reader.TokenType == JsonToken.Date)
            {
                if (!DateTime.TryParse(reader.Value.ToString(), out DateTime dt))
                    throw new Exception(string.Format("Error converting value {0} to type '{1}'", reader.Value, objectType));

                return (long)dt.Subtract(utc_1970).TotalSeconds;
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                //数值
                return Convert.ToInt64(reader.Value);
            }

            throw new Exception(string.Format("Unexpected token {0} when parsing enum,Value is {1}", reader.TokenType, reader.ToString()));
        }

        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value);
        }

        /// <summary>
        /// IsNullableType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsNullableType(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            if (t.IsGenericType)
                return (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
            else
                return t.BaseType.FullName == "System.ValueType";
        }
    }
}