using System;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesType Encoder
    /// </summary>
    public class BytesTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly IntTypeEncoder _intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BytesTypeEncoder()
        {
            _intTypeEncoder = new IntTypeEncoder();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            //default to false, this is a byte array we are not responsible for endianism
            return Encode(value, false);
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            if (!(value is byte[]))
                throw new Exception("byte[] value expected for type 'bytes'");

            return (byte[])value;
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkEndian"></param>
        /// <returns></returns>
        public byte[] Encode(object value, bool checkEndian)
        {
            if (!(value is byte[]))
                throw new Exception("byte[] value expected for type 'bytes'");

            byte[] bb = (byte[]) value;
            byte[] ret = new byte[((bb.Length - 1)/32 + 1)*32]; // padding 32 bytes

            //It should always be Big Endian.
            if (BitConverter.IsLittleEndian && checkEndian)
                bb = bb.Reverse().ToArray();

            Array.Copy(bb, 0, ret, 0, bb.Length);

            return ByteUtil.Merge(_intTypeEncoder.EncodeInt(bb.Length), ret);
        }

        #endregion
    }
}