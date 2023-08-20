using System.Collections.Generic;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// Omni scan client interface
    /// </summary>
    public interface IOmniScanClient
    {
        /// <summary>
        /// Returns the balance for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        Dictionary<string, OmniBtcBalanceJson> GetAddressBTC(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns the balance information for a given address. 
        /// For multiple addresses in a single query use the v2 endpoint
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniAssetCollectionJson GetAddressV1(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns the balance information for multiple addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        Dictionary<string, OmniAssetCollectionJson> GetAddressV2(string[] addresses, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns the balance information and transaction history list for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniAddressDetailsResponse GetAddressDetails(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Return a list of currently active/available base currencies the omnidex 
        /// has open orders against. 
        /// Data: 
        ///     ecosystem : 
        ///         1 for main / production ecosystem 
        ///         or 
        ///         2 for test/development ecosystem
        /// </summary>
        /// <param name="ecosystem"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniDesignatingCurrenciesResponse DesignatingCurrencies(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns list of transactions (up to 10 per page) relevant to queried Property ID. 
        /// Returned transaction types include: 
        /// Creation Tx, Change issuer txs, Grant Txs, Revoke Txs, Crowdsale Participation Txs, 
        /// Close Crowdsale earlier tx
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="page"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniTxHistoryResponse GetHistory(int propertyId, int page = 1, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Return list of properties created by a queried address.
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniListByOwnerResponse ListByOwner(string[] addresses, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns list of currently active crowdsales. 
        /// Data: 
        ///     ecosystem : 
        ///         1 for production/main ecosystem. 
        ///         2 for test/dev ecosystem
        /// </summary>
        /// <param name="ecosystem"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniCrowdSalesResponse ListActiveCrowdSales(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// returns list of created properties filtered by ecosystem. 
        /// Data: 
        ///     ecosystem : 
        ///         1 for production/main ecosystem. 
        ///         2 for test/dev ecosystem
        /// </summary>
        /// <param name="ecosystem"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniListByEcosystemResponse ListByEcosystem(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns list of all created properties.
        /// </summary>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniCoinListResponse PropertyList(OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Search by transaction id, address or property id. 
        /// Data: 
        ///     query : 
        ///         text string of either Transaction ID, Address, or property id to search for
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniSearchResponse Search(string query, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns list of transactions for queried address. 
        /// Data: 
        ///     addr : 
        ///         address to query page : 
        ///             cycle through available response pages (10 txs per page)
        /// </summary>
        /// <param name="address"></param>
        /// <param name="page"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniTransactionListResponse GetTxList(string address, int page = 0, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Broadcast a signed transaction to the network. 
        /// Data: 
        ///     signedTransaction : signed hex to broadcast
        /// </summary>
        /// <param name="signedTransaction"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniPushTxResponse PushTx(string signedTransaction, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// Returns transaction details of a queried transaction hash.
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        OmniTxInfoResponse GetTx(string txHash, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10);
    }
}