namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Rest API
    /// </summary>
    public interface ITronNetRest : 
        ITronNetAddressUtilitiesRest, 
        ITronNetAccountRest, 
        ITronNetAccountResourcesRest,
        ITronNetTransactionsRest, 
        ITronNetQueryNetworkRest, 
        ITronNetTRC10TokenRest, 
        ITronNetSmartContractsRest
    {

    }
}
