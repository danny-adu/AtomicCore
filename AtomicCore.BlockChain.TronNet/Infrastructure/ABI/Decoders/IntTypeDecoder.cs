using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// IntType Decoder
    /// </summary>
    public class IntTypeDecoder : TypeDecoder
    {
        #region Variables

        private const string HEX_PREFIX = "0x";
        private readonly bool _signed;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public IntTypeDecoder() : this(false)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signed"></param>
        public IntTypeDecoder(bool signed)
        {
            _signed = signed;
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
            if (type == typeof(byte))
                return DecodeByte(encoded);

            if (type == typeof(sbyte))
                return DecodeSbyte(encoded);

            if (type == typeof(short))
                return DecodeShort(encoded);

            if (type == typeof(ushort))
                return DecodeUShort(encoded);

            if (type == typeof(int))
                return DecodeInt(encoded);

            if (type.GetTypeInfo().IsEnum)
            {
                var val = DecodeInt(encoded);
                return Enum.ToObject(type, val);
            }

            if (type == typeof(uint))
                return DecodeUInt(encoded);

            if (type == typeof(long))
                return DecodeLong(encoded);

            if (type == typeof(ulong))
                return DecodeULong(encoded);

            if ((type == typeof(BigInteger)) || (type == typeof(object)))
                return DecodeBigInteger(encoded);

            throw new NotSupportedException(type + " is not a supported decoding type for IntType");
        }

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(BigInteger);
        }

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return (type == typeof(int)) || (type == typeof(uint)) ||
                   (type == typeof(ulong)) || (type == typeof(long)) ||
                   (type == typeof(short)) || (type == typeof(ushort)) ||
                   (type == typeof(byte)) || (type == typeof(sbyte)) ||
                   (type == typeof(BigInteger)) || (type == typeof(object))
                   || type.GetTypeInfo().IsEnum;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode Big Integer
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public BigInteger DecodeBigInteger(string hexString)
        {
            if (!hexString.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase))
                hexString = string.Format("{0}{1}", HEX_PREFIX, hexString);

            return DecodeBigInteger(hexString.HexToByteArray());
        }

        /// <summary>
        /// Decode Big Integer
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public BigInteger DecodeBigInteger(byte[] encoded)
        {
            var negative = false;
            if (_signed) negative = encoded.First() == 0xFF;

            if (!_signed)
            {
                var listEncoded = encoded.ToList();
                listEncoded.Insert(0, 0x00);
                encoded = listEncoded.ToArray();
            }

            if (BitConverter.IsLittleEndian)
                encoded = encoded.Reverse().ToArray();

            if (negative)
                return new BigInteger(encoded) -
                       new BigInteger(
                           "0xffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff".HexToByteArray()) - 1;

            return new BigInteger(encoded);
        }

        /// <summary>
        /// DecodeByte
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public byte DecodeByte(byte[] encoded)
        {
            return (byte)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeSbyte
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public sbyte DecodeSbyte(byte[] encoded)
        {
            return (sbyte)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeShort
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public short DecodeShort(byte[] encoded)
        {
            return (short)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeUShort
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public ushort DecodeUShort(byte[] encoded)
        {
            return (ushort)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeInt
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public int DecodeInt(byte[] encoded)
        {
            return (int)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeLong
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public long DecodeLong(byte[] encoded)
        {
            return (long)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeUInt
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public uint DecodeUInt(byte[] encoded)
        {
            return (uint)DecodeBigInteger(encoded);
        }

        /// <summary>
        /// DecodeULong
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public ulong DecodeULong(byte[] encoded)
        {
            return (ulong)DecodeBigInteger(encoded);
        }

        #endregion
    }
}