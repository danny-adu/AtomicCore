using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BoolType Decoder
    /// </summary>
    public class BoolTypeDecoder : TypeDecoder
    {
        #region Variables

        private readonly IntTypeDecoder _intTypeDecoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BoolTypeDecoder()
        {
            _intTypeDecoder = new IntTypeDecoder();
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

            int decoded = _intTypeDecoder.DecodeInt(encoded);

            return Convert.ToBoolean(decoded);
        }

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(bool);
        }

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return (type == typeof(bool)) || (type == typeof(object));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool Decode(byte[] encoded)
        {
            return Decode<bool>(encoded);
        }

        #endregion
    }
}