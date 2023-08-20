namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ABIValue
    /// </summary>
    public class ABIValue
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="abiType"></param>
        /// <param name="value"></param>
        public ABIValue(ABIType abiType, object value)
        {
            ABIType = abiType;
            Value = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="abiType"></param>
        /// <param name="value"></param>
        public ABIValue(string abiType, object value)
        {
            ABIType = ABIType.CreateABIType(abiType);
            Value = value;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ABIType
        /// </summary>
        public ABIType ABIType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}