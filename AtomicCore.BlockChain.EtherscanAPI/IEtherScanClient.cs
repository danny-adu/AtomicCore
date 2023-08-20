namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// IEtherScanClient interface definition
    /// </summary>
    public interface IEtherScanClient : IEtherAccounts, IEtherContracts, IEtherTransactions, IEtherBlocks, IEtherLogs, IEtherProxy, IEtherTokens, IEtherGasTracker, IEtherStats
    {

    }
}
