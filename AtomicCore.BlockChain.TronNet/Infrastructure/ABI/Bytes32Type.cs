namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Bytes32Type
    /// </summary>
    public class Bytes32Type : ABIType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public Bytes32Type(string name) : base(name)
        {
            Decoder = new Bytes32TypeDecoder();
            Encoder = new Bytes32TypeEncoder();
        }
    }
}