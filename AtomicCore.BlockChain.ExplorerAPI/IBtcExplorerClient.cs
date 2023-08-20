using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// IBlockChainExplorer
    /// </summary>
    /// <remarks>
    /// https://www.blockchain.com/api/blockchain_api
    /// </remarks>
    public interface IBtcExplorerClient
    {
        /// <summary>
        /// Get Single Block By Hash
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        BtcSingleBlockResponse GetSingleBlock(string blockHash, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None);

        /// <summary>
        /// UnspentOutputs
        /// </summary>
        /// <param name="address">Address can be base58 or xpub</param>
        /// <param name="limit">Optional limit parameter to show n transactions e.g. limit=50 (Default: 250, Max: 1000)</param>
        /// <param name="confirmations">Optional confirmations parameter to limit the minimum confirmations e.g. confirmations=6</param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        BtcUnspentOutputResponse UnspentOutputs(string address, int? limit = null, int? confirmations = null, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None);

        /// <summary>
        /// Get Address Balance(BTC)
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        BtcAddressBalanceResponse GetAddressBalance(string address, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None);

        /// <summary>
        ///  Get Address Txs
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        BtcAddressTxsResponse GetAddressTxs(string address, int offset = 0, int limit = 0, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None);
    }
}
