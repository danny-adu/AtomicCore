using System;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StringType Encoder
    /// </summary>
    public class StringTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly BytesTypeEncoder byteTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public StringTypeEncoder()
        {
            byteTypeEncoder = new BytesTypeEncoder();
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
            string valueStr = value as string;
            if (string.IsNullOrEmpty(valueStr))
                throw new Exception("String value expected for type 'string'");

            if(string.IsNullOrEmpty(valueStr))
                throw new Exception("String value is null");

            byte[] bytes = Encoding.UTF8.GetBytes(valueStr);

            return byteTypeEncoder.Encode(bytes, false);
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            string valueStr = value as string;
            if (string.IsNullOrEmpty(valueStr))
                throw new Exception("String value expected for type 'string'");

            if (string.IsNullOrEmpty(valueStr))
                throw new Exception("String value is null");

            return Encoding.UTF8.GetBytes(valueStr);
        }

        #endregion
    }
}