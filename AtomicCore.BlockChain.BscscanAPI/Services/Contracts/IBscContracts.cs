namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscContracts interface
    /// </summary>
    public interface IBscContracts
    {
        /// <summary>
        /// Returns the contract Application Binary Interface ( ABI ) of a verified smart contract.
        /// </summary>
        /// <param name="contractAddress">the contract address that has a verified source code</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetContractABI(string contractAddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
