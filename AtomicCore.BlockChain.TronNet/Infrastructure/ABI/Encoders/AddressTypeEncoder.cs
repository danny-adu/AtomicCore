using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// AddressType Encoder
    /// </summary>
    public class AddressTypeEncoder : ITypeEncoder
    {
        #region Variables

        private const string HEX_PREFIX = "0x";
        private readonly IntTypeEncoder _intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructors
        /// </summary>
        public AddressTypeEncoder()
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
            string strValue = value as string;
            if (string.IsNullOrEmpty(strValue)) throw new Exception("Invalid type for address expected as string");

            if (
                !string.IsNullOrEmpty(strValue) &&
                !strValue.StartsWith(HEX_PREFIX, StringComparison.Ordinal)
            )
                strValue = string.Format("{0}{1}", HEX_PREFIX, strValue);

            byte[] addr = _intTypeEncoder.Encode(strValue);

            for (var i = 0; i < 12; i++)
            {
                if ((addr[i] != 0) && (addr[i] != 0xFF))
                    throw new Exception("Invalid address (should be 20 bytes length): " + addr.ToHex());

                if (addr[i] == 0xFF) addr[i] = 0;
            }

            return addr;
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            string strValue = value as string;
            if (string.IsNullOrEmpty(strValue)) throw new Exception("Invalid type for address expected as string");

            if (
                !string.IsNullOrEmpty(strValue) && 
                !strValue.StartsWith(HEX_PREFIX, StringComparison.Ordinal)
            )
                strValue = string.Format("{0}{1}", HEX_PREFIX, strValue);

            if (strValue.Length == 42) return strValue.HexToByteArray();

            throw new Exception("Invalid address (should be 20 bytes length): " + strValue);
        }

        #endregion
    }
}