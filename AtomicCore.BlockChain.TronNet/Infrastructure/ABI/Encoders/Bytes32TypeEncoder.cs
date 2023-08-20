using System;
using System.Numerics;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Bytes32Type Encoder
    /// </summary>
    public class Bytes32TypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly IntTypeEncoder _intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Bytes32TypeEncoder()
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
            if (value.IsNumber())
            {
                var bigInt = BigInteger.Parse(value.ToString());
                return _intTypeEncoder.EncodeInt(bigInt);
            }

            if (value is string stringValue)
            {
                byte[] returnBytes = new byte[32];
                byte[] bytes = Encoding.UTF8.GetBytes(stringValue);
                if (bytes.Length > 32) throw new ArgumentException("After retrieving the UTF8 bytes for the string, it is longer than 32 bytes, please using the string type, or a byte array if the string was a hex value");
                Array.Copy(bytes, 0, returnBytes, 0, bytes.Length);

                return returnBytes;
            }

            if (value is byte[] bytesValue)
            {
                if (bytesValue.Length > 32) throw new ArgumentException("Expected byte array no bigger than 32 bytes");

                byte[] returnArray = new byte[((bytesValue.Length - 1) / 32 + 1) * 32];
                Array.Copy(bytesValue, 0, returnArray, 0, bytesValue.Length);

                return returnArray;
            }

            throw new ArgumentException("Expected Numeric Type or String to be Encoded as Bytes32");
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            return Encode(value);
        }

        #endregion
    }
}