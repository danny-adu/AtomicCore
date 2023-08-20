namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BoolType
    /// </summary>
    public class BoolType : ABIType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BoolType() : base("bool")
        {
            Decoder = new BoolTypeDecoder();
            Encoder = new BoolTypeEncoder();
        }
    }
}