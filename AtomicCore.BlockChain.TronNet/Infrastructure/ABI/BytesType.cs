namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesType
    /// </summary>
    public class BytesType : ABIType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BytesType() : base("bytes")
        {
            Decoder = new BytesTypeDecoder();
            Encoder = new BytesTypeEncoder();
        }

        /// <summary>
        /// FixedSize
        /// </summary>
        public override int FixedSize => -1;
    }
}