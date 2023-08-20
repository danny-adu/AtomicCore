using System;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StringType Decoder
    /// </summary>
    public class StringTypeDecoder : TypeDecoder
    {
        #region override Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object Decode(byte[] encoded, Type type)
        {
            if (!IsSupportedType(type)) throw new NotSupportedException(type + " is not supported");
            return Encoding.UTF8.GetString(encoded, 32, EncoderDecoderHelpers.GetNumberOfBytes(encoded));
        }

        /// <summary>
        /// Get Default DecodingType
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(string);
        }

        /// <summary>
        /// Is SupportedType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return (type == typeof(string)) || (type == typeof(object));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public string Decode(byte[] encoded)
        {
            return Decode<string>(encoded);
        }

        #endregion
    }
}