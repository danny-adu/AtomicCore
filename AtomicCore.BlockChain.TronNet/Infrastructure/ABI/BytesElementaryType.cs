namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BytesElementaryType
    /// </summary>
    public class BytesElementaryType : ABIType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public BytesElementaryType(string name, int size) : base(name)
        {
            Decoder = new BytesElementaryTypeDecoder(size);
            Encoder = new BytesElementaryTypeEncoder(size);
        }
    }
}