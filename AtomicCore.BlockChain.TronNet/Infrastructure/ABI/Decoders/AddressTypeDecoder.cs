using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// AddressType Decoder
    /// </summary>
    public class AddressTypeDecoder : TypeDecoder
    {
        #region Variables

        /// <summary>
        /// IntType Decoder
        /// </summary>
        private readonly IntTypeDecoder _intTypeDecoder = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AddressTypeDecoder()
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
            byte[] output = new byte[20];
            Array.Copy(encoded, 12, output, 0, 20);

            return output.ToHex(true);
        }

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(string);
        }

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return (type == typeof(string)) || (type == typeof(object));
        }

        #endregion
    }
}