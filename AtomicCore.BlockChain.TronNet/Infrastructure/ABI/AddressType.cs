namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// AddressType
    /// </summary>
    public class AddressType : ABIType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddressType() : base("address")
        {
            //this will need to be only a string type one, converting to hex
            Decoder = new AddressTypeDecoder();
            Encoder = new AddressTypeEncoder();
        }
    }
}