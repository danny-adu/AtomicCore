using System;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Bytes32Type Decoder
    /// </summary>
    public class Bytes32TypeDecoder : TypeDecoder
    {
        #region Variables

        private readonly BoolTypeDecoder _boolTypeDecoder;
        private readonly IntTypeDecoder _intTypeDecoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Bytes32TypeDecoder()
        {
            _intTypeDecoder = new IntTypeDecoder();
            _boolTypeDecoder = new BoolTypeDecoder();
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

            if ((type == typeof(byte[])) || (type == typeof(object)))
                return encoded;

            if (type == typeof(string))
                return DecodeString(encoded);

            if (_intTypeDecoder.IsSupportedType(type))
                return _intTypeDecoder.Decode(encoded, type);

            if (_boolTypeDecoder.IsSupportedType(type))
                return _boolTypeDecoder.Decode(encoded, type);

            throw new NotSupportedException();
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
            return (type == typeof(byte[])) || (type == typeof(string)) || _intTypeDecoder.IsSupportedType(type)
                   || (type == typeof(bool)) || (type == typeof(object));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Decode String
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        private static string DecodeString(byte[] encoded)
        {
            return Encoding.UTF8.GetString(encoded, 0, encoded.Length).TrimEnd('\0');
        }

        #endregion
    }
}