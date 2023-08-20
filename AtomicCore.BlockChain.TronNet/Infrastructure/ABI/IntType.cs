using System;
using System.Numerics;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// IntType
    /// </summary>
    public class IntType : ABIType
    {
        #region Variables

        public static readonly BigInteger MAX_INT256_VALUE = BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819967");
        public static readonly BigInteger MIN_INT256_VALUE = BigInteger.Parse("-57896044618658097711785492504343953926634992332820282019728792003956564819968");
        public static readonly BigInteger MAX_UINT256_VALUE = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935");
        public static readonly BigInteger MIN_UINT_VALUE = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public IntType(string name) : base(name)
        {
            Decoder = new IntTypeDecoder(IsSigned(CanonicalName));
            Encoder = new IntTypeEncoder(IsSigned(CanonicalName), GetSize(CanonicalName));
        }

        #endregion

        #region Propertys

        /// <summary>
        /// CanonicalName
        /// </summary>
        public override string CanonicalName
        {
            get
            {
                if (Name.Equals("int"))
                    return "int256";
                if (Name.Equals("uint"))
                    return "uint256";

                return base.CanonicalName;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// IsSigned
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool IsSigned(string name)
        {
            return !name.ToLower().StartsWith("u");
        }

        /// <summary>
        /// GetMaxSignedValue
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BigInteger GetMaxSignedValue(uint size)
        {
            CheckIsValidAndThrow(size);
            return BigInteger.Pow(2, (int)size - 1) - 1;
        }

        /// <summary>
        /// GetMinSignedValue
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BigInteger GetMinSignedValue(uint size)
        {
            CheckIsValidAndThrow(size);
            return BigInteger.Pow(-2, (int)size - 1);
        }

        /// <summary>
        /// GetMaxUnSignedValue
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static BigInteger GetMaxUnSignedValue(uint size)
        {
            CheckIsValidAndThrow(size);
            return BigInteger.Pow(2, (int)size) - 1;
        }

        /// <summary>
        /// CheckIsValidAndThrow
        /// </summary>
        /// <param name="size"></param>
        private static void CheckIsValidAndThrow(uint size)
        {
            if (!IsValidSize(size)) throw new ArgumentException("Invalid size for type int :" + size);
        }

        /// <summary>
        /// IsValidSize
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool IsValidSize(uint size)
        {
            var divisible = (size % 8 == 0);
            return divisible && size <= 256 && size >= 8;
        }

        /// <summary>
        /// GetSize
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static uint GetSize(string name)
        {
            if (IsSigned(name))
                return uint.Parse(name.Substring(3));
            else
                return uint.Parse(name.Substring(4));
        }

        #endregion
    }
}