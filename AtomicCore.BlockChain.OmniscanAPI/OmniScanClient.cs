using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni scan client service
    /// </summary>
    public class OmniScanClient : IOmniScanClient
    {
        #region Variables

        /// <summary>
        /// api rest base url
        /// </summary>
        private const string C_APIREST_BASEURL = "https://api.omniwallet.org";

        /// <summary>
        /// agent url tmp
        /// </summary>
        private readonly string _agentGetTmp;

        /// <summary>
        /// agent url tmp
        /// </summary>
        private readonly string _agentPostTmp;

        #endregion

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="agentGetTmp"></param>
        /// <param name="agentPostTmp"></param>
        public OmniScanClient(string agentGetTmp = null, string agentPostTmp = null)
        {
            _agentGetTmp = agentGetTmp;
            _agentPostTmp = agentPostTmp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// create rest url
        /// </summary>
        /// <param name="version">version</param>
        /// <param name="actionUrl">action url</param>
        /// <returns></returns>
        private string CreateRestUrl(OmniRestVersion version, string actionUrl)
        {
            return $"{C_APIREST_BASEURL}/{version}/{actionUrl}".ToLower();
        }

        /// <summary>
        /// rest get request
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        private string RestGet(string url)
        {
            string resp;
            try
            {
                if (string.IsNullOrEmpty(this._agentGetTmp))
                    resp = HttpProtocol.HttpGet(url);
                else
                {
                    string encodeUrl = UrlEncoder.UrlEncode(url);
                    string remoteUrl = string.Format(this._agentGetTmp, encodeUrl);

                    resp = HttpProtocol.HttpGet(remoteUrl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// rest get request(Using HttpClient)
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        private string RestGet2(string url)
        {
            string resp;
            try
            {
                string remoteUrl;
                if (string.IsNullOrEmpty(this._agentGetTmp))
                    remoteUrl = url;
                else
                {
                    string encodeUrl = UrlEncoder.UrlEncode(url);
                    remoteUrl = string.Format(this._agentGetTmp, encodeUrl);
                }

                using HttpClient cli = new HttpClient();
                HttpResponseMessage respMessage = cli.GetAsync(remoteUrl).Result;
                if (!respMessage.IsSuccessStatusCode)
                    throw new HttpRequestException($"StatusCode -> {respMessage.StatusCode}, ");

                resp = respMessage.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// rest post request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string RestPost(string url, string data)
        {
            string resp;
            try
            {
                if (string.IsNullOrEmpty(this._agentPostTmp))
                    resp = HttpProtocol.HttpPost(url, data, HttpProtocol.XWWWFORMURLENCODED);
                else
                {
                    string encodeUrl = UrlEncoder.UrlEncode(url);
                    string remoteUrl = string.Format(this._agentPostTmp, url, encodeUrl);

                    resp = HttpProtocol.HttpPost(url, data, HttpProtocol.XWWWFORMURLENCODED);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// json -> check error propertys
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private string HasResponseError(string resp)
        {
            JObject json_obj;
            try
            {
                json_obj = JObject.Parse(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (json_obj.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken error_json))
                return error_json.ToString();

            return null;
        }

        /// <summary>
        /// json -> object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private T ObjectParse<T>(string resp)
            where T : class, new()
        {
            T jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        #endregion

        #region IOmniScanClient Methods

        /// <summary>
        /// Returns the balance for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public Dictionary<string, OmniBtcBalanceJson> GetAddressBTC(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetAddressBTCNoCache(address);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetAddressBTC), address);
                bool exists = OmniCacheProvider.Get(cacheKey, out Dictionary<string, OmniBtcBalanceJson> cacheData);
                if (!exists)
                {
                    cacheData = GetAddressBTCNoCache(address);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the balance information for a given address. 
        /// For multiple addresses in a single query use the v2 endpoint
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniAssetCollectionJson GetAddressV1(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetAddressV1NoCache(address);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetAddressV1), address);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniAssetCollectionJson cacheData);
                if (!exists)
                {
                    cacheData = GetAddressV1NoCache(address);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the balance information for multiple addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public Dictionary<string, OmniAssetCollectionJson> GetAddressV2(string[] addresses, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetAddressV2NoCache(addresses);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetAddressV2), addresses);
                bool exists = OmniCacheProvider.Get(cacheKey, out Dictionary<string, OmniAssetCollectionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetAddressV2NoCache(addresses);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the balance information and transaction history list for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniAddressDetailsResponse GetAddressDetails(string address, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetAddressDetailsNoCache(address);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetAddressDetails), address);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniAddressDetailsResponse cacheData);
                if (!exists)
                {
                    cacheData = GetAddressDetailsNoCache(address);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniDesignatingCurrenciesResponse DesignatingCurrencies(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return DesignatingCurrenciesNoCache(ecosystem);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(DesignatingCurrencies), ecosystem.ToString());
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniDesignatingCurrenciesResponse cacheData);
                if (!exists)
                {
                    cacheData = DesignatingCurrenciesNoCache(ecosystem);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniTxHistoryResponse GetHistory(int propertyId, int page = 0, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetHistoryNoCache(propertyId, page);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetHistory), propertyId.ToString(), page.ToString());
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniTxHistoryResponse cacheData);
                if (!exists)
                {
                    cacheData = GetHistoryNoCache(propertyId, page);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Return list of properties created by a queried address.
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniListByOwnerResponse ListByOwner(string[] addresses, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return ListByOwnerNoCache(addresses);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(ListByOwner), addresses);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniListByOwnerResponse cacheData);
                if (!exists)
                {
                    cacheData = ListByOwnerNoCache(addresses);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniCrowdSalesResponse ListActiveCrowdSales(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return ListActiveCrowdSalesNoCache(ecosystem);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(ListActiveCrowdSales), ecosystem.ToString());
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniCrowdSalesResponse cacheData);
                if (!exists)
                {
                    cacheData = ListActiveCrowdSalesNoCache(ecosystem);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniListByEcosystemResponse ListByEcosystem(int ecosystem, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return ListByEcosystemNoCache(ecosystem);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(ListByEcosystem), ecosystem.ToString());
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniListByEcosystemResponse cacheData);
                if (!exists)
                {
                    cacheData = ListByEcosystemNoCache(ecosystem);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns list of all created properties.
        /// </summary>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniCoinListResponse PropertyList(OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return PropertyListNoCache();
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(PropertyList));
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniCoinListResponse cacheData);
                if (!exists)
                {
                    cacheData = PropertyListNoCache();

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniSearchResponse Search(string query, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return SearchNoCache(query);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(Search), query);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniSearchResponse cacheData);
                if (!exists)
                {
                    cacheData = SearchNoCache(query);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

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
        public OmniTransactionListResponse GetTxList(string address, int page = 0, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetTxListNoCache(address, page);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetTxList), address);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniTransactionListResponse cacheData);
                if (!exists)
                {
                    cacheData = GetTxListNoCache(address, page);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Broadcast a signed transaction to the network. 
        /// Data: 
        ///     signedTransaction : signed hex to broadcast
        /// </summary>
        /// <param name="signedTransaction"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniPushTxResponse PushTx(string signedTransaction, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return PushTxNoCache(signedTransaction);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(PushTx), signedTransaction);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniPushTxResponse cacheData);
                if (!exists)
                {
                    cacheData = PushTxNoCache(signedTransaction);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns transaction details of a queried transaction hash.
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public OmniTxInfoResponse GetTx(string txHash, OmniCacheMode cacheMode = OmniCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == OmniCacheMode.None)
                return GetTxNoCache(txHash);
            else
            {
                string cacheKey = OmniCacheProvider.GenerateCacheKey(nameof(GetTx), txHash);
                bool exists = OmniCacheProvider.Get(cacheKey, out OmniTxInfoResponse cacheData);
                if (!exists)
                {
                    cacheData = GetTxNoCache(txHash);

                    OmniCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the balance for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private Dictionary<string, OmniBtcBalanceJson> GetAddressBTCNoCache(string address)
        {
            if (null == address || address.Length <= 0)
                throw new ArgumentNullException(nameof(address));

            string url = $"https://blockchain.info/balance?cors=true&active={address}";
            string resp = RestGet2(url);

            return ObjectParse<Dictionary<string, OmniBtcBalanceJson>>(resp);
        }

        /// <summary>
        /// Returns the balance information for a given address. 
        /// For multiple addresses in a single query use the v2 endpoint
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private OmniAssetCollectionJson GetAddressV1NoCache(string address)
        {
            if (null == address || address.Length <= 0)
                throw new ArgumentNullException(nameof(address));

            string data = $"addr={address}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "address/addr/");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniAssetCollectionJson>(resp);
        }

        /// <summary>
        /// Returns the balance information for multiple addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        private Dictionary<string, OmniAssetCollectionJson> GetAddressV2NoCache(string[] addresses)
        {
            if (null == addresses || addresses.Length <= 0)
                throw new ArgumentNullException(nameof(addresses));

            string data = string.Join("&", addresses.Select(s => $"addr={s}"));
            string url = this.CreateRestUrl(OmniRestVersion.V2, "address/addr/");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<Dictionary<string, OmniAssetCollectionJson>>(resp);
        }

        /// <summary>
        /// Returns the balance information and transaction history list for a given address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private OmniAddressDetailsResponse GetAddressDetailsNoCache(string address)
        {
            if (null == address || address.Length <= 0)
                throw new ArgumentNullException(nameof(address));

            string data = $"addr={address}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "address/addr/details/");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniAddressDetailsResponse>(resp);
        }

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
        /// <returns></returns>
        private OmniDesignatingCurrenciesResponse DesignatingCurrenciesNoCache(int ecosystem)
        {
            if (ecosystem != 1 && ecosystem != 2)
                throw new ArgumentOutOfRangeException("1 for main / production ecosystem or 2 for test/development ecosystem");

            string data = $"ecosystem={ecosystem}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "omnidex/designatingcurrencies");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniDesignatingCurrenciesResponse>(resp);
        }

        /// <summary>
        /// Returns list of transactions (up to 10 per page) relevant to queried Property ID. 
        /// Returned transaction types include: 
        /// Creation Tx, Change issuer txs, Grant Txs, Revoke Txs, Crowdsale Participation Txs, 
        /// Close Crowdsale earlier tx
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private OmniTxHistoryResponse GetHistoryNoCache(int propertyId, int page = 0)
        {
            if (propertyId <= 0)
                throw new ArgumentOutOfRangeException(nameof(propertyId));
            if (page < 0)
                page = 0;

            string data = $"page={page}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, $"properties/gethistory/{propertyId}");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniTxHistoryResponse>(resp);
        }

        /// <summary>
        /// Return list of properties created by a queried address.
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        private OmniListByOwnerResponse ListByOwnerNoCache(params string[] addresses)
        {
            if (null == addresses || addresses.Length <= 0)
                throw new ArgumentNullException(nameof(addresses));

            string data = string.Join("&", addresses.Select(s => $"addresses={s}"));
            string url = this.CreateRestUrl(OmniRestVersion.V1, $"properties/listbyowner");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniListByOwnerResponse>(resp);
        }

        /// <summary>
        /// Returns list of currently active crowdsales. 
        /// Data: 
        ///     ecosystem : 
        ///         1 for production/main ecosystem. 
        ///         2 for test/dev ecosystem
        /// </summary>
        /// <param name="ecosystem"></param>
        /// <returns></returns>
        private OmniCrowdSalesResponse ListActiveCrowdSalesNoCache(int ecosystem)
        {
            if (ecosystem != 1 && ecosystem != 2)
                throw new ArgumentOutOfRangeException("1 for main / production ecosystem or 2 for test/development ecosystem");

            string data = $"ecosystem={ecosystem}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "properties/listactivecrowdsales");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniCrowdSalesResponse>(resp);
        }

        /// <summary>
        /// returns list of created properties filtered by ecosystem. 
        /// Data: 
        ///     ecosystem : 
        ///         1 for production/main ecosystem. 
        ///         2 for test/dev ecosystem
        /// </summary>
        /// <param name="ecosystem"></param>
        /// <returns></returns>
        private OmniListByEcosystemResponse ListByEcosystemNoCache(int ecosystem)
        {
            if (ecosystem != 1 && ecosystem != 2)
                throw new ArgumentOutOfRangeException("1 for main / production ecosystem or 2 for test/development ecosystem");

            string data = $"ecosystem={ecosystem}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "properties/listbyecosystem");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniListByEcosystemResponse>(resp);
        }

        /// <summary>
        /// Returns list of all created properties.
        /// </summary>
        /// <returns></returns>
        private OmniCoinListResponse PropertyListNoCache()
        {
            string url = this.CreateRestUrl(OmniRestVersion.V1, "properties/list");
            string resp = this.RestGet(url);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniCoinListResponse>(resp);
        }

        /// <summary>
        /// Search by transaction id, address or property id. 
        /// Data: 
        ///     query : 
        ///         text string of either Transaction ID, Address, or property id to search for
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private OmniSearchResponse SearchNoCache(string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));

            string data = $"query={query}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "search");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniSearchResponse>(resp);
        }

        /// <summary>
        /// Returns list of transactions for queried address. 
        /// Data: 
        ///     addr : 
        ///         address to query page : 
        ///             cycle through available response pages (10 txs per page)
        /// </summary>
        /// <param name="address"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private OmniTransactionListResponse GetTxListNoCache(string address, int page = 0)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            string data = $"addr={address}&page={page}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "transaction/address");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniTransactionListResponse>(resp);
        }

        /// <summary>
        /// Broadcast a signed transaction to the network. 
        /// Data: 
        ///     signedTransaction : signed hex to broadcast
        /// </summary>
        /// <param name="signedTransaction"></param>
        /// <returns></returns>
        private OmniPushTxResponse PushTxNoCache(string signedTransaction)
        {
            if (string.IsNullOrEmpty(signedTransaction))
                throw new ArgumentNullException(nameof(signedTransaction));

            string data = $"signedTransaction={signedTransaction}";
            string url = this.CreateRestUrl(OmniRestVersion.V1, "transaction/pushtx/");
            string resp = this.RestPost(url, data);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniPushTxResponse>(resp);
        }

        /// <summary>
        /// Returns transaction details of a queried transaction hash.
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        private OmniTxInfoResponse GetTxNoCache(string txHash)
        {
            if (string.IsNullOrEmpty(txHash))
                throw new ArgumentNullException(nameof(txHash));

            string url = this.CreateRestUrl(OmniRestVersion.V1, $"transaction/tx/{txHash}");
            string resp = this.RestGet(url);

            string error = HasResponseError(resp);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return ObjectParse<OmniTxInfoResponse>(resp);
        }

        #endregion
    }
}
