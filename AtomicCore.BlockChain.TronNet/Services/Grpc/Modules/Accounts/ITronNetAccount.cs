namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Account Interface
    /// </summary>
    public interface ITronNetAccount
    {
        /// <summary>
        /// Public Key
        /// </summary>
        string PublicKey { get; }

        /// <summary>
        /// Private Key
        /// </summary>
        string PrivateKey { get; }

        /// <summary>
        /// Current Address
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Get Address Prefix(eg:0x41)
        /// </summary>
        /// <returns></returns>
        byte GetAddressPrefix();
    }
}
