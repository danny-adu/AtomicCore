using System;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesElementaryType Encoder
    /// </summary>
    public class BytesElementaryTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly int _size;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        public BytesElementaryTypeEncoder(int size)
        {
            if(size > 32) throw new ArgumentException("bytes(Number) for an elementary type can only be a Maximum of 32");
            this._size = size;
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
            if (_size == 1 && value is byte @byte)
                value = new byte[1] { @byte };

            if (_size == 16 && value is Guid guid)
                value = guid.ToByteArray();

            if (!(value is byte[]))
                throw new Exception("byte[] value expected for type 'bytes'");

            byte[] byteArray = (byte[])value;
            if (byteArray.Length != _size)
                throw new Exception("byte[] size expected to be " + _size);

            return byteArray;

        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkEndian"></param>
        /// <returns></returns>
        public byte[] Encode(object value, bool checkEndian)
        {
            if(_size == 1 && value is byte @byte)
                value = new byte[1] { @byte };

            if (_size == 16 && value is Guid guid)
                value = guid.ToByteArray();

            if (!(value is byte[]))
                throw new Exception("byte[] value expected for type 'bytes'");

            byte[] byteArray = (byte[])value;
            if (byteArray.Length != _size)
                throw new Exception("byte[] size expected to be " + _size);

            // padding 32 bytes
            byte[] returnArray = new byte[((byteArray.Length - 1) / 32 + 1) * 32]; 

            //It should always be Big Endian.
            if (BitConverter.IsLittleEndian && checkEndian)
                byteArray = byteArray.Reverse().ToArray();

            Array.Copy(byteArray, 0, returnArray, 0, byteArray.Length);

            return returnArray;
        }

        #endregion
    }
}