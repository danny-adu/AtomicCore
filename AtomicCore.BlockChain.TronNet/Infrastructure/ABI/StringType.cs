namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StringType
    /// </summary>
    public class StringType : ABIType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public StringType() : base("string")
        {
            Decoder = new StringTypeDecoder();
            Encoder = new StringTypeEncoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// FixedSize
        /// </summary>
        public override int FixedSize => -1;

        #endregion
    }
}