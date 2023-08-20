using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// BlockChain Explorer
    /// </summary>
    public class BtcExplorerClient : BaseExplorerClient, IBtcExplorerClient
    {
        #region Variables

        /// <summary>
        /// cache seconds short(10 seconds)
        /// </summary>
        private const int c_cacheSeconds_short = 10;

        /// <summary>
        /// Blockchain Data API, eg : https://blockchain.info
        /// </summary>
        private const string C_BLOCKCHAIN_INFOS = "https://blockchain.info";

        /// <summary>
        /// api rest base url, eg : https://api.blockchain.info
        /// </summary>
        private const string C_APIREST_BASEURL = "https://api.blockchain.info";

        #endregion

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="agentGetTmp"></param>
        /// <param name="agentPostTmp"></param>
        public BtcExplorerClient(string agentGetTmp = null, string agentPostTmp = null)
            : base(agentGetTmp, agentPostTmp)
        {

        }

        #endregion

        #region IBlockChainExplorer

        /// <summary>
        /// Get Block By Hash
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        public BtcSingleBlockResponse GetSingleBlock(string blockHash, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None)
        {
            if (string.IsNullOrEmpty(blockHash))
                throw new ArgumentNullException(nameof(blockHash));
            if (64 != blockHash.Length)
                throw new ArgumentException("illegal blockHash parameter format");

            BtcSingleBlockResponse resultData = null;
            if (cacheSeconds <= 0 || ExplorerAPICacheMode.None == cacheMode)
            {
                string url = $"{C_BLOCKCHAIN_INFOS}/rawblock/{blockHash}";
                string resp = RestGet(url);

                resultData = ObjectParse<BtcSingleBlockResponse>(resp);
                resultData.DebugUrl = url;
            }
            else
            {
                string cacheKey = ApiMsCacheProvider.GenerateCacheKey(nameof(GetSingleBlock), blockHash);
                bool exists = ApiMsCacheProvider.Get(cacheKey, out resultData);
                if (!exists)
                {
                    string url = $"{C_BLOCKCHAIN_INFOS}/rawblock/{blockHash}";
                    string resp = RestGet(url);

                    resultData = ObjectParse<BtcSingleBlockResponse>(resp);
                    resultData.DebugUrl = url;

                    ApiMsCacheProvider.Set(cacheKey, resultData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }
            }

            return resultData;
        }

        /// <summary>
        /// UnspentOutputs
        /// </summary>
        /// <param name="address">Address can be base58 or xpub</param>
        /// <param name="limit">Optional limit parameter to show n transactions e.g. limit=50 (Default: 250, Max: 1000)</param>
        /// <param name="confirmations">Optional confirmations parameter to limit the minimum confirmations e.g. confirmations=6</param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        public BtcUnspentOutputResponse UnspentOutputs(string address, int? limit = null, int? confirmations = null, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            if (34 != address.Length)
                throw new ArgumentException("illegal address parameter format");
            if (null != limit)
                if (limit.Value > 1000)
                    limit = 1000;

            BtcUnspentOutputResponse resultData = null;
            if (cacheSeconds <= 0 || ExplorerAPICacheMode.None == cacheMode)
            {
                StringBuilder urlBuilder = new StringBuilder($"{C_BLOCKCHAIN_INFOS}/unspent?active={address}");
                if (null != limit && limit.Value > 0)
                    urlBuilder.Append($"&limit={limit.Value}");
                if (null != confirmations && confirmations.Value > 0)
                    urlBuilder.Append($"&confirmations={confirmations.Value}");

                string url = urlBuilder.ToString();
                string resp = RestGet(url);

                resultData = ObjectParse<BtcUnspentOutputResponse>(resp);
                resultData.DebugUrl = url;
            }
            else
            {
                string cacheKey = ApiMsCacheProvider.GenerateCacheKey(nameof(UnspentOutputs), address);
                bool exists = ApiMsCacheProvider.Get(cacheKey, out resultData);
                if (!exists)
                {
                    StringBuilder urlBuilder = new StringBuilder($"{C_BLOCKCHAIN_INFOS}/unspent?active={address}");
                    if (null != limit && limit.Value > 0)
                        urlBuilder.Append($"&limit={limit.Value}");
                    if (null != confirmations && confirmations.Value > 0)
                        urlBuilder.Append($"&confirmations={confirmations.Value}");

                    string url = urlBuilder.ToString();
                    string resp = RestGet(url);

                    resultData = ObjectParse<BtcUnspentOutputResponse>(resp);
                    resultData.DebugUrl = url;

                    ApiMsCacheProvider.Set(cacheKey, resultData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }
            }

            return resultData;
        }

        /// <summary>
        /// Get Address Balance(BTC)
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        public BtcAddressBalanceResponse GetAddressBalance(string address, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            if (34 != address.Length)
                throw new ArgumentException("illegal address parameter format");

            BtcAddressBalanceResponse resultData = null;
            if (cacheSeconds <= 0 || ExplorerAPICacheMode.None == cacheMode)
            {
                string url = $"{C_APIREST_BASEURL}/haskoin-store/btc/address/{address}/balance";
                string resp = RestGet(url);

                resultData = ObjectParse<BtcAddressBalanceResponse>(resp);
                resultData.DebugUrl = url;
            }
            else
            {
                string cacheKey = ApiMsCacheProvider.GenerateCacheKey(nameof(GetAddressBalance), address);
                bool exists = ApiMsCacheProvider.Get(cacheKey, out resultData);
                if (!exists)
                {
                    string url = $"{C_APIREST_BASEURL}/haskoin-store/btc/address/{address}/balance";
                    string resp = RestGet(url);

                    resultData = ObjectParse<BtcAddressBalanceResponse>(resp);
                    resultData.DebugUrl = url;

                    ApiMsCacheProvider.Set(cacheKey, resultData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }
            }

            return resultData;
        }

        /// <summary>
        /// Get Address Txs
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="cacheSeconds"></param>
        /// <param name="cacheMode"></param>
        /// <returns></returns>
        public BtcAddressTxsResponse GetAddressTxs(string address, int offset = 0, int limit = 0, int cacheSeconds = 0, ExplorerAPICacheMode cacheMode = ExplorerAPICacheMode.None)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            if (34 != address.Length)
                throw new ArgumentException("illegal address parameter format");

            BtcAddressTxsResponse resultData = null;
            if (cacheSeconds <= 0 || ExplorerAPICacheMode.None == cacheMode)
            {
                List<string> paramList = new List<string>();
                if (offset > 0)
                    paramList.Add($"offset={offset}");
                if (limit > 0)
                    paramList.Add($"limit={limit}");

                string url = $"{C_APIREST_BASEURL}/haskoin-store/btc/address/{address}/transactions/full{(paramList.Count > 0 ? $"?{string.Join("&", paramList)}" : string.Empty)}";
                string resp = RestGet(url);

                resultData = ObjectParse<BtcAddressTxsResponse>(resp);
                resultData.DebugUrl = url;
            }
            else
            {
                string cacheKey = ApiMsCacheProvider.GenerateCacheKey(nameof(GetAddressTxs), address, offset.ToString(), limit.ToString());
                bool exists = ApiMsCacheProvider.Get(cacheKey, out resultData);
                if (!exists)
                {
                    List<string> paramList = new List<string>();
                    if (offset > 0)
                        paramList.Add($"offset={offset}");
                    if (limit > 0)
                        paramList.Add($"limit={limit}");

                    string url = $"{C_APIREST_BASEURL}/haskoin-store/btc/address/{address}/transactions/full{(paramList.Count > 0 ? $"?{string.Join("&", paramList)}" : string.Empty)}";
                    string resp = RestGet(url);

                    resultData = ObjectParse<BtcAddressTxsResponse>(resp);
                    resultData.DebugUrl = url;

                    ApiMsCacheProvider.Set(cacheKey, resultData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }
            }

            return resultData;
        }

        #endregion
    }
}
