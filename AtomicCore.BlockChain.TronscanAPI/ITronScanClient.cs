namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// TRON SCAN INTERFACE
    /// </summary>
    public interface ITronScanClient
    {
        /// <summary>
        /// 1.Block Overview
        /// </summary>
        /// <returns></returns>
        TronChainOverviewJson BlockOverview();

        /// <summary>
        /// 2.Get Last Block
        /// </summary>
        /// <returns></returns>
        TronBlockBasicJson GetLastBlock();

        /// <summary>
        /// 3.List all the accounts in the blockchain
        /// only 10,000 accounts are displayed, sorted by TRX balance from high to low
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronChainTopAddressListJson GetChainTopAddress(int start = 0, int limit = 20, string sort = "-balance");

        /// <summary>
        /// 4.Get Account Assets list
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        TronAccountAssetJson GetAccountAssets(string address);

        /// <summary>
        /// 5.List the blocks in the blockchain
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronBlockInfoListJson GetLastBlocks(int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-number");

        /// <summary>
        /// 6.List all the blocks produced by the specified SR in the blockchain
        /// </summary>
        /// <param name="srAddress">SR address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronBlockInfoListJson GetSRBlocks(string srAddress, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-number");

        /// <summary>
        /// 7.Get a single block's detail
        /// </summary>
        /// <param name="number">block number</param>
        /// <returns></returns>
        TronBlockDetailsJson GetBlockByNumber(ulong number);

        /// <summary>
        /// 8.Get Last Transaction List
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronNormalTransactionListJson GetLastTransactions(int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp");

        /// <summary>
        /// 9.List the transactions related to a specified account
        /// </summary>
        /// <param name="address">address: an account(No address specified means all)</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronNormalTransactionListJson GetNormalTransactions(string address = null, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp");

        /// <summary>
        /// 10.List the transactions related to an smart contract
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="contract">contract: contract address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronContractTransactionListJson GetContractTransactions(string contract, int start = 0, int limit = 20, bool count = true, string sort = "-timestamp");

        /// <summary>
        /// 11.List a transaction detail
        /// </summary>
        /// <param name="txHash">query transaction hash</param>
        /// <returns></returns>
        TronTransactionInfoJson GetTransactionByHash(string txHash);

        /// <summary>
        /// 12.List the transfers in the blockchain
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronNormalTransferListJson GetLastTransfers(int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp");

        /// <summary>
        /// 13.List the transfers related to an specified account
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="address">transfers related address</param>
        /// <param name="token">'_' shows only TRX transfers</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <param name="count">total number of records</param>
        /// <param name="sort">define the sequence of the records return</param>
        /// <returns></returns>
        TronNormalTransferListJson GetNormalTransfers(string address = null, string token = null, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null, bool count = true, string sort = "-timestamp");

        /// <summary>
        /// 14.List all the nodes in the blockchain
        /// </summary>
        /// <returns></returns>
        TronChainNodeListJson GetChainNodes();

        /// <summary>
        /// 20.Get a single contract's abi and byteCode
        /// </summary>
        /// <param name="contract">contract address</param>
        /// <returns></returns>
        TronContractABICodeJson GetContractABI(string contract);

        /// <summary>
        /// 39.List the TRC-20 transfers related to a specified account
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="address">an account</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns></returns>
        TronTRC20TransferEventListJson GetTRC20TransferEvents(string address, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null);

        /// <summary>
        /// 40.List the internal transactions related to a specified account
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="address">an account</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns></returns>
        TronInternalTransactionListJson GetInternalTransactions(string address, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null);

        /// <summary>
        /// 41.List the transfers related to a specified TRC10 token(Order by Desc)
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="issueAddress">token creation address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="name">token name</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns>TRC10 token transfers list</returns>
        TronTRC10TransactionListJson GetTRC10Transactions(string issueAddress, int start = 0, int limit = 20, string name = null, ulong? start_timestamp = null, ulong? end_timestamp = null);

        /// <summary>
        /// 42.List the transfers related to a specified TRC20 token
        /// only display the latest 10,000 data records in the query time range
        /// </summary>
        /// <param name="contractAddress">contract address</param>
        /// <param name="start">query index for pagination</param>
        /// <param name="limit">page size for pagination</param>
        /// <param name="start_timestamp">query date range</param>
        /// <param name="end_timestamp">query date range</param>
        /// <returns>TRC20 token transfers list</returns>
        TronTRC20TransactionListJson GetTRC20Transactions(string contractAddress, int start = 0, int limit = 20, ulong? start_timestamp = null, ulong? end_timestamp = null);

        /// <summary>
        /// Get Resource Transaction List
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        /// <param name="resourceType"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        TronResourceTransactionListJson GetResourceTransaction(string address, TronResourceTarget type = TronResourceTarget.None, TronResourceType resourceType = TronResourceType.None, int start = 0, int limit = 500);
    }
}
