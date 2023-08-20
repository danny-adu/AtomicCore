namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscTransactions interface
    /// </summary>
    public interface IBscTransactions
    {
        /// <summary>
        /// Returns the status code of a transaction execution.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check the execution status</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscTransactionReceiptStatusJson> GetTransactionReceiptStatus(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
