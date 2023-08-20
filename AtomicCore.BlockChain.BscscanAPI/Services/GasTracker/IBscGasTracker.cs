namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscGasTracker Interface
    /// </summary>
    public interface IBscGasTracker
    {
        /// <summary>
        /// Returns the current Safe, Proposed and Fast gas prices. 
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscGasOracleJson> GetGasOracle(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
