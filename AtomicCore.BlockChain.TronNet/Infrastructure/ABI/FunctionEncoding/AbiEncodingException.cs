using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// AbiEncoding Exception
    /// </summary>
    public class AbiEncodingException : Exception
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="order"></param>
        /// <param name="abiType"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AbiEncodingException(int order, ABIType abiType, object value, string message, Exception innerException) : base(message, innerException)
        {
            Order = order;
            ABIType = abiType;
            Value = value;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// ABIType
        /// </summary>
        public ABIType ABIType { get; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; }

        #endregion
    }
}

