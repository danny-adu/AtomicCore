namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Decoded Value
    /// </summary>
    public class DecodedValue
    {
        /// <summary>
        /// AbiName
        /// </summary>
        public string AbiName { get; set; }

        /// <summary>
        /// AbiType
        /// </summary>
        public string AbiType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
    }
}