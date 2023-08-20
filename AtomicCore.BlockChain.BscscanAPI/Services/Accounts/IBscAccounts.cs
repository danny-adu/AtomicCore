using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscAccounts Interface
    /// </summary>
    public interface IBscAccounts
    {
        /// <summary>
        /// Get Balance
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<decimal> GetBalance(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Get Balance Raw
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BigInteger> GetBalanceRaw(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Get Balance List
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscAccountBalanceJson> GetBalanceList(string[] address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of transactions performed by an address, with optional pagination.
        ///     Note : This API endpoint returns a maximum of 10000 records only.
        ///     Tip: Specify a smaller startblock and endblock range for faster search results.
        /// </summary>
        /// <param name="address">the string representing the addresses to check for balance</param>
        /// <param name="startblock">the integer block number to start searching for transactions</param>
        /// <param name="endblock">the integer block number to stop searching for transactions</param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscNormalTransactionJson> GetNormalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of internal transactions performed by an address, with optional pagination.
        ///     Note : This API endpoint returns a maximum of 10000 records only.
        ///     Tip: Specify a smaller startblock and endblock range for faster search results
        /// </summary>
        /// <param name="address">the string representing the addresses to check for balance</param>
        /// <param name="startblock">the integer block number to start searching for transactions</param>
        /// <param name="endblock">the integer block number to stop searching for transactions</param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscInternalTransactionJson> GetInternalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of internal transactions performed within a transaction.
        ///     Note : This API endpoint returns a maximum of 10000 records only.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check for internal transactions</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscInternalEventJson> GetInternalTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of BEP-20 tokens transferred by an address, with optional filtering by token contract.
        /// Usage:
        ///         BEP-20 transfers from an address, specify the address parameter
        ///         BEP-20 transfers from a contract address, specify the contract address parameter
        ///         BEP-20 transfers from an address filtered by a token contract, specify both address and contract address parameters.
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="contractaddress">the string representing the token contract address to check for balance</param>
        /// <param name="startblock">the integer block number to start searching for transactions</param>
        /// <param name="endblock">the integer block number to stop searching for transactions</param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscBEP20TransactionJson> GetBEP20TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of BEP-721 ( NFT ) tokens transferred by an address, with optional filtering by token contract.
        /// Usage:
        ///         BEP-721 transfers from an address, specify the address parameter 
        ///         BEP-721 transfers from a contract address, specify the contract address parameter
        ///         BEP-721 transfers from an address filtered by a token contract, specify both address and contract address parameters.
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="contractaddress">the string representing the token contract address to check for balance</param>
        /// <param name="startblock">the integer block number to start searching for transactions</param>
        /// <param name="endblock">the integer block number to stop searching for transactions</param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscBEP721TransactionJson> GetBEP721TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the list of blocks validated by an address.
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="blocktype">the string pre-defined block type, blocksfor canonical blocks </param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscMineRewardJson> GetMinedBlockListByAddress(string address, string blocktype, int page = 1, int offset = 10000, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
