using System;
using System.Linq;
using System.Numerics;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// IntType Encoder
    /// </summary>
    public class IntTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly IntTypeDecoder intTypeDecoder;
        private readonly bool _signed;
        private readonly uint _size;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public IntTypeEncoder() : this(false, 256)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signed"></param>
        /// <param name="size"></param>
        public IntTypeEncoder(bool signed, uint size)
        {
            intTypeDecoder = new IntTypeDecoder();
            _signed = signed;
            _size = size;
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
            return Encode(value, 32);
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfBytesArray"></param>
        /// <returns></returns>
        public byte[] Encode(object value, uint numberOfBytesArray)
        {
            BigInteger bigInt;

            if (value is string stringValue)
                bigInt = intTypeDecoder.Decode<BigInteger>(stringValue);
            else if (value is BigInteger integer)
                bigInt = integer;
            else if (value.IsNumber())
                bigInt = BigInteger.Parse(value.ToString());
            else if (value is Enum)
                bigInt = (int)value;
            else
                throw new Exception($"Invalid value for type '{this}'. Value: {value ?? "null"}, ValueType: ({value?.GetType()})");

            return EncodeInt(bigInt, numberOfBytesArray);
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            return Encode(value, _size / 8);
        }

        /// <summary>
        /// EncodeInt
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodeInt(int value)
        {
            return EncodeInt(new BigInteger(value));
        }

        /// <summary>
        /// EncodeInt
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfBytesArray"></param>
        /// <returns></returns>
        public byte[] EncodeInt(BigInteger value, uint numberOfBytesArray)
        {
            ValidateValue(value);

            //It should always be Big Endian.
            byte[] bytes = BitConverter.IsLittleEndian
                ? value.ToByteArray().Reverse().ToArray()
                : value.ToByteArray();

            if (bytes.Length == 33 && !_signed)
                if (bytes[0] == 0x00)
                    bytes = bytes.Skip(1).ToArray();
                else
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Unsigned SmartContract integer must not exceed maximum value for uint256: {IntType.MAX_UINT256_VALUE}. Current value is: {value}");

            byte[] ret = new byte[numberOfBytesArray];
            for (var i = 0; i < ret.Length; i++)
                if (value.Sign < 0)
                    ret[i] = 0xFF;
                else
                    ret[i] = 0;

            Array.Copy(bytes, 0, ret, (int)numberOfBytesArray - bytes.Length, bytes.Length);

            return ret;
        }

        /// <summary>
        /// EncodeInt
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodeInt(BigInteger value)
        {
            return EncodeInt(value, 32);
        }

        /// <summary>
        /// ValidateValue
        /// </summary>
        /// <param name="value"></param>
        public void ValidateValue(BigInteger value)
        {
            if (_signed && value > IntType.GetMaxSignedValue(_size)) throw new ArgumentOutOfRangeException(nameof(value),
                $"Signed SmartContract integer must not exceed maximum value for int{_size}: {IntType.GetMaxSignedValue(_size)}. Current value is: {value}");

            if (_signed && value < IntType.GetMinSignedValue(_size)) throw new ArgumentOutOfRangeException(nameof(value),
                $"Signed SmartContract integer must not be less than the minimum value for int{_size}: {IntType.GetMinSignedValue(_size)}. Current value is: {value}");

            if (!_signed && value > IntType.GetMaxUnSignedValue(_size)) throw new ArgumentOutOfRangeException(nameof(value),
                $"Unsigned SmartContract integer must not exceed maximum value for uint{_size}: {IntType.GetMaxUnSignedValue(_size)}. Current value is: {value}");

            if (!_signed && value < IntType.MIN_UINT_VALUE) throw new ArgumentOutOfRangeException(nameof(value),
                $"Unsigned SmartContract integer must not be less than the minimum value of uint: {IntType.MIN_UINT_VALUE}. Current value is: {value}");
        }

        #endregion
    }
}