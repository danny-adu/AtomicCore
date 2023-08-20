namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscGethProxy Interface
    /// </summary>
    public interface IBscGethProxy
    {
        /// <summary>
        /// Returns the number of most recent block
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<long> GetBlockNumber(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscRpcBlockSimpleJson> GetBlockSimpleByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscRpcBlockFullJson> GetBlockFullByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the number of transactions in a block.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<int> GetBlockTransactionCountByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns information about a transaction requested by transaction hash.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscRpcTransactionJson> GetTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns information about a transaction by block number and transaction index position.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="index">the position of the uncle's index in the block, in hex eg. 0x1</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscRpcTransactionJson> GetTransactionByBlockNumberAndIndex(long blockNumber, int index, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the number of transactions performed by an address.
        /// </summary>
        /// <param name="address">the string representing the address to get transaction count</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<int> GetTransactionCount(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Submits a pre-signed transaction for broadcast to the Binance Smart Chain network.
        /// </summary>
        /// <param name="hex">the string representing the signed raw transaction data to broadcast.</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        BscscanSingleResult<string> SendRawTransaction(string hex, BscNetwork network = BscNetwork.BscMainnet);

        /// <summary>
        /// Returns the receipt of a transaction that has been validated.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscRpcTransactionReceiptJson> GetTransactionReceipt(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Executes a new message call immediately without creating a transaction on the block chain.
        /// </summary>
        /// <param name="to">the string representing the address to interact with</param>
        /// <param name="data">the hash of the method signature and encoded parameters</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> Call(string to, string data, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns code at a given address.
        /// </summary>
        /// <param name="address">the string representing the address to get code</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetCode(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the value from a storage position at a given address.
        /// </summary>
        /// <param name="address">the string representing the address to get code</param>
        /// <param name="position">the hex code of the position in storage, eg 0x0</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<string> GetStorageAt(string address, string position, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the current price per gas in wei.
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<long> GasPrice(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Makes a call or transaction, which won't be added to the blockchain and returns the gas used. 
        /// </summary>
        /// <param name="data">the hash of the method signature and encoded parameters</param>
        /// <param name="to">the string representing the address to interact with</param>
        /// <param name="value">the value sent in this transaction, in hex eg. 0xff22</param>
        /// <param name="gasPrice">the gas price paid for each unit of gas, in wei</param>
        /// <param name="gas">the amount of gas provided for the transaction, in hex eg. 0x5f5e0ff</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<long> EstimateGas(string data, string to, string value, string gasPrice, string gas, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
