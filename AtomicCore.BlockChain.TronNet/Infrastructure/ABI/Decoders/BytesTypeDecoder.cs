using System;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesType Decoder
    /// </summary>
    public class BytesTypeDecoder : TypeDecoder
    {
        #region Variables

        private readonly StringTypeDecoder _stringTypeDecoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BytesTypeDecoder()
        {
            _stringTypeDecoder = new StringTypeDecoder();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object Decode(byte[] encoded, Type type)
        {
            if (!IsSupportedType(type)) throw new NotSupportedException(type + " is not supported");

            if (type == typeof(string)) return _stringTypeDecoder.Decode(encoded, type);

            byte[] returnArray = encoded.Skip(32).Take(EncoderDecoderHelpers.GetNumberOfBytes(encoded)).ToArray();

            if (type == typeof(byte))
                return returnArray[0];
            
            return returnArray;
        }

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(byte[]);
        }

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return 
                (type == typeof(string)) || 
                (type == typeof(byte[])) || 
                (type == typeof(object)) || 
                (type == typeof(byte));
        }

        #endregion
    }
}