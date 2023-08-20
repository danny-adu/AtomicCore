using System;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesElementaryType Decoder
    /// </summary>
    public class BytesElementaryTypeDecoder : TypeDecoder
    {
        #region Variables

        private readonly int _size;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        public BytesElementaryTypeDecoder(int size)
        {
            this._size = size;
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

            byte[] returnArray = encoded.Take(_size).ToArray();

            if (_size == 1 && type == typeof(byte))
                return returnArray[0];

            if (_size == 16 && type == typeof(Guid))
                return new Guid(returnArray);

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
            if (_size == 1) return (type == typeof(byte[]) || type == typeof(byte));
            if (_size == 16) return (type == typeof(byte[]) || type == typeof(Guid));

            return (type == typeof(byte[]));
        }

        #endregion
    }
}