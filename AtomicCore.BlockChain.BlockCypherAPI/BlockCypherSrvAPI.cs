using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// BlockCypher Service
    /// </summary>
    public class BlockCypherSrvAPI : IBlockCypherAPI
    {
        #region Variables

        /// <summary>
        /// application/json
        /// </summary>
        private const string APPLICATIONJSON = "application/json";

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
        public BlockCypherSrvAPI(string agentGetTmp = null, string agentPostTmp = null)
        {
            _agentGetTmp = agentGetTmp;
            _agentPostTmp = agentPostTmp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 根据网络获取基础请求地址
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private string GetBaseUrl(BlockCypherNetwork network)
        {
            string baseUrl;
            switch (network)
            {
                case BlockCypherNetwork.BtcMainnet:
                    baseUrl = "http://api.blockcypher.com/v1/btc/main";
                    break;
                case BlockCypherNetwork.BtcTestnet:
                    baseUrl = "http://api.blockcypher.com/v1/btc/test3";
                    break;
                case BlockCypherNetwork.DashMainnet:
                    baseUrl = "http://api.blockcypher.com/v1/dash/main";
                    break;
                case BlockCypherNetwork.DogeMainnet:
                    baseUrl = "http://api.blockcypher.com/v1/doge/main";
                    break;
                case BlockCypherNetwork.LiteMainnet:
                    baseUrl = "http://api.blockcypher.com/v1/ltc/main";
                    break;
                case BlockCypherNetwork.BcyMainnet:
                    baseUrl = "http://api.blockcypher.com/v1/bcy/test";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return baseUrl;
        }

        /// <summary>
        /// create rest url
        /// </summary>
        /// <param name="network">version</param>
        /// <param name="actionUrl">action url</param>
        /// <returns></returns>
        private string GetRestUrl(BlockCypherNetwork network, string actionUrl)
        {
            string baseUrl = GetBaseUrl(network);
            return $"{baseUrl}/{actionUrl}".ToLower();
        }

        /// <summary>
        /// rest get request(Using HttpClient)
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        private string RestGet(string url)
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
                HttpResponseMessage response = cli.GetAsync(remoteUrl).Result;
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"StatusCode -> {response.StatusCode}, ");

                resp = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// rest post request(Using HttpClient)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string RestPost<T>(string url, T data)
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
                HttpResponseMessage response = cli.PostAsync(remoteUrl, new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    APPLICATIONJSON
                )).Result;

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"StatusCode -> {response.StatusCode}, ");

                resp = response.Content.ReadAsStringAsync().Result;
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

        #region NoCache Methods

        /// <summary>
        /// General information about a blockchain is available by GET-ing the base resource.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#blockchain-api
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private ChainEndpointResponse ChainEndpointNoCache(BlockCypherNetwork network)
        {
            string url = GetBaseUrl(network);
            string resp = RestGet(url);

            return ObjectParse<ChainEndpointResponse>(resp);
        }

        /// <summary>
        /// If you want more data on a particular block, you can use the Block Hash endpoint.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#block-hash-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="blockHash"></param>
        private BlockCypherBlockResponse BlockHashEndpointNoCache(BlockCypherNetwork network, string blockHash)
        {
            if (string.IsNullOrEmpty(blockHash))
                throw new ArgumentNullException(nameof(blockHash));

            string url = GetRestUrl(network, $"blocks/{blockHash}");
            string resp = RestGet(url);

            return ObjectParse<BlockCypherBlockResponse>(resp);
        }

        /// <summary>
        /// You can also query for information on a block using its height, using the same resource but with a different variable type.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#block-height-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="blockHeight"></param>
        private BlockCypherBlockResponse BlockHeightEndpointNoCache(BlockCypherNetwork network, string blockHeight)
        {
            if (string.IsNullOrEmpty(blockHeight))
                throw new ArgumentNullException(nameof(blockHeight));

            string url = GetRestUrl(network, $"blocks/{blockHeight}");
            string resp = RestGet(url);

            return ObjectParse<BlockCypherBlockResponse>(resp);
        }


        /// <summary>
        /// The Address Balance Endpoint is the simplest---and fastest---method to get a subset of information on a public address.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-balance-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="addressBalance"></param>
        private AddressEndpointResponse AddressBalanceEndpointNoCache(BlockCypherNetwork network, string addressBalance)
        {
            if (string.IsNullOrEmpty(addressBalance))
                throw new ArgumentNullException(nameof(addressBalance));

            string url = GetRestUrl(network, $"addrs/{addressBalance}/balance");
            string resp = RestGet(url);

            return ObjectParse<AddressEndpointResponse>(resp);
        }

        /// <summary>
        ///The default Address Endpoint strikes a balance between speed of response and data on Addresses. It returns more information about an address' transactions than the Address Balance Endpoint but doesn't return full transaction information (like the Address Full Endpoint).
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="address"></param>
        private AddressEndpointResponse AddressEndpointNoCache(BlockCypherNetwork network, string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            string url = GetRestUrl(network, $"addrs/{address}");
            string resp = RestGet(url);

            return ObjectParse<AddressEndpointResponse>(resp);
        }

        /// <summary>
        ///The Address Full Endpoint returns all information available about a particular address, including an array of complete transactions instead of just transaction inputs and outputs. Unfortunately, because of the amount of data returned, it is the slowest of the address endpoints, but it returns the most detailed data record.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-full-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="addressFull"></param>
        private AddressEndpointResponse AddressFullEndpointNoCache(BlockCypherNetwork network, string addressFull)
        {
            if (string.IsNullOrEmpty(addressFull))
                throw new ArgumentNullException(nameof(addressFull));

            string url = GetRestUrl(network, $"addrs/{addressFull}/full");
            string resp = RestGet(url);

            return ObjectParse<AddressEndpointResponse>(resp);
        }

        /// <summary>
        ///The Generate Address endpoint allows you to generate private-public key-pairs along with an associated public address. No information is required with this POST request.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#generate-address-endpoint
        /// </summary>
        /// <param name="network"></param>
        private AddressKeychainEndpointResponse GenerateAddressEndpointNoCache(BlockCypherNetwork network)
        {
            string url = GetBaseUrl(network);
            string resp = RestGet(url);

            return ObjectParse<AddressKeychainEndpointResponse>(resp);
        }

        /// <summary>
        ///The Generate Multisig Address Endpoint is a convenience method to help you generate multisig addresses from multiple public keys. After supplying a partially filled-out AddressKeychain object (including only an array of hex-encoded public keys and the script type), the returned object includes the computed public address.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#generate-multisig-address-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data">BlockCypherAddressKeychainJson</param>
        private AddressKeychainEndpointResponse GenerateMultisigAddressEndpointNoCache(BlockCypherNetwork network, BlockCypherAddressKeychainJson data)
        {
            string url = GetBaseUrl(network);
            string resp = RestPost(url, data);

            return ObjectParse<AddressKeychainEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#create-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data">BlockCypherWalletJson</param>
        private WalletEndpointResponse CreateWalletEndpointNoCache(BlockCypherNetwork network, BlockCypherWalletJson data)
        {
            string url = GetBaseUrl(network);
            string resp = RestPost(url, data);

            return ObjectParse<WalletEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#create-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data">BlockCypherHDWalletJson</param>
        private HDWalletEndpointResponse CreateHDWalletEndpointNoCache(BlockCypherNetwork network, BlockCypherHDWalletJson data)
        {
            string url = GetBaseUrl(network);
            string resp = RestPost(url, data);

            return ObjectParse<HDWalletEndpointResponse>(resp);
        }

        /// <summary>
        ///This endpoint returns a string array ($NAMEARRAY) of active wallet names (both normal and HD) under the token you queried. You can then query detailed information on individual wallets (via their names) by leveraging the Get Wallet Endpoint.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#list-wallets-endpoint
        /// </summary>
        /// <param name="network"></param>
        private ListWalletsEndpointResponse ListWalletsEndpointNoCache(BlockCypherNetwork network)
        {
            string url = GetBaseUrl(network);
            string resp = RestGet(url);

            return ObjectParse<ListWalletsEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        private WalletEndpointResponse GetWalletEndpointNoCache(BlockCypherNetwork network, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string url = GetRestUrl(network, $"wallets/{name}");
            string resp = RestGet(url);

            return ObjectParse<WalletEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        private HDWalletEndpointResponse GetHDWalletEndpointNoCache(BlockCypherNetwork network, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string url = GetRestUrl(network, $"wallets/{name}");
            string resp = RestGet(url);

            return ObjectParse<HDWalletEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#add-addresses-to-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        private WalletEndpointResponse AddAddressesToWalletEndpointNoCache(BlockCypherNetwork network, string name, BlockCypherWalletJson data)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string url = GetRestUrl(network, $"wallets/{name}/addresses");
            string resp = RestPost(url,data);

            return ObjectParse<WalletEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-addresses-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        private WalletEndpointResponse GetWalletAddressesEndpointNoCache(BlockCypherNetwork network, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string url = GetRestUrl(network, $"wallets/{name}/addresses");
            string resp = RestGet(url);

            return ObjectParse<WalletEndpointResponse>(resp);
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-addresses-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        private HDWalletEndpointResponse GetHDWalletAddressesEndpointNoCache(BlockCypherNetwork network, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string url = GetRestUrl(network, $"wallets/hd/{name}/addresses");
            string resp = RestGet(url);

            return ObjectParse<HDWalletEndpointResponse>(resp);
        }
        #endregion

        #region IBlockCypherAPI Methods

        /// <summary>
        /// General information about a blockchain is available by GET-ing the base resource.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#blockchain-api
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public ChainEndpointResponse ChainEndpoint(BlockCypherNetwork network, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return ChainEndpointNoCache(network);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(ChainEndpoint), network.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out ChainEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = ChainEndpointNoCache(network);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// If you want more data on a particular block, you can use the Block Hash endpoint.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#block-hash-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="blockHash"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public BlockCypherBlockResponse BlockHashEndpoint(BlockCypherNetwork network, string blockHash, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return BlockHashEndpointNoCache(network, blockHash);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(BlockHashEndpoint), blockHash);
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out BlockCypherBlockResponse cacheData);
                if (!exists)
                {
                    cacheData = BlockHashEndpointNoCache(network, blockHash);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// You can also query for information on a block using its height, using the same resource but with a different variable type.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#block-height-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="blockHeight"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public BlockCypherBlockResponse BlockHeightEndpoint(BlockCypherNetwork network, string blockHeight, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return BlockHeightEndpointNoCache(network, blockHeight);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(BlockHeightEndpoint), blockHeight);
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out BlockCypherBlockResponse cacheData);
                if (!exists)
                {
                    cacheData = BlockHeightEndpointNoCache(network, blockHeight);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// The Address Balance Endpoint is the simplest---and fastest---method to get a subset of information on a public address.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-balance-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="addressBalance"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public AddressEndpointResponse AddressBalanceEndpoint(BlockCypherNetwork network, string addressBalance, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return AddressBalanceEndpointNoCache(network, addressBalance);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(AddressBalanceEndpoint), addressBalance);
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out AddressEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = AddressBalanceEndpointNoCache(network, addressBalance);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        ///The default Address Endpoint strikes a balance between speed of response and data on Addresses. It returns more information about an address' transactions than the Address Balance Endpoint but doesn't return full transaction information (like the Address Full Endpoint).
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="address"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public AddressEndpointResponse AddressEndpoint(BlockCypherNetwork network, string address, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return AddressEndpointNoCache(network, address);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(AddressEndpoint), address);
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out AddressEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = AddressEndpointNoCache(network, address);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        ///The Address Full Endpoint returns all information available about a particular address, including an array of complete transactions instead of just transaction inputs and outputs. Unfortunately, because of the amount of data returned, it is the slowest of the address endpoints, but it returns the most detailed data record.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#address-full-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="addressFull"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public AddressEndpointResponse AddressFullEndpoint(BlockCypherNetwork network, string addressFull, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return AddressFullEndpointNoCache(network, addressFull);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(AddressFullEndpoint), addressFull);
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out AddressEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = AddressFullEndpointNoCache(network, addressFull);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        ///The Generate Address endpoint allows you to generate private-public key-pairs along with an associated public address. No information is required with this POST request.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#generate-address-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public AddressKeychainEndpointResponse GenerateAddressEndpoint(BlockCypherNetwork network, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GenerateAddressEndpointNoCache(network);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GenerateAddressEndpoint), network.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out AddressKeychainEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GenerateAddressEndpointNoCache(network);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        ///The Generate Multisig Address Endpoint is a convenience method to help you generate multisig addresses from multiple public keys. After supplying a partially filled-out AddressKeychain object (including only an array of hex-encoded public keys and the script type), the returned object includes the computed public address.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#generate-multisig-address-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public AddressKeychainEndpointResponse GenerateMultisigAddressEndpoint(BlockCypherNetwork network, BlockCypherAddressKeychainJson data, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GenerateMultisigAddressEndpointNoCache(network, data);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GenerateMultisigAddressEndpoint), Newtonsoft.Json.JsonConvert.SerializeObject(data).ToLower());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out AddressKeychainEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GenerateMultisigAddressEndpointNoCache(network, data);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#create-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public WalletEndpointResponse CreateWalletEndpoint(BlockCypherNetwork network, BlockCypherWalletJson data, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return CreateWalletEndpointNoCache(network, data);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(CreateWalletEndpoint), Newtonsoft.Json.JsonConvert.SerializeObject(data).ToLower());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out WalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = CreateWalletEndpointNoCache(network, data);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#create-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="data"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public HDWalletEndpointResponse CreateHDWalletEndpoint(BlockCypherNetwork network, BlockCypherHDWalletJson data, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return CreateHDWalletEndpointNoCache(network, data);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(CreateHDWalletEndpoint), Newtonsoft.Json.JsonConvert.SerializeObject(data).ToLower());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out HDWalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = CreateHDWalletEndpointNoCache(network, data);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        ///This endpoint returns a string array ($NAMEARRAY) of active wallet names (both normal and HD) under the token you queried. You can then query detailed information on individual wallets (via their names) by leveraging the Get Wallet Endpoint.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#list-wallets-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public ListWalletsEndpointResponse ListWalletsEndpoint(BlockCypherNetwork network, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return ListWalletsEndpointNoCache(network);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(ListWalletsEndpoint), network.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out ListWalletsEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = ListWalletsEndpointNoCache(network);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public WalletEndpointResponse GetWalletEndpoint(BlockCypherNetwork network, string name, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GetWalletEndpointNoCache(network, name);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GetWalletEndpoint), name.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out WalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GetWalletEndpointNoCache(network, name);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public HDWalletEndpointResponse GetHDWalletEndpoint(BlockCypherNetwork network, string name, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GetHDWalletEndpointNoCache(network, name);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GetHDWalletEndpoint), name.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out HDWalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GetHDWalletEndpointNoCache(network, name);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#add-addresses-to-wallet-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public WalletEndpointResponse AddAddressesToWalletEndpoint(BlockCypherNetwork network, string name, BlockCypherWalletJson data, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return AddAddressesToWalletEndpointNoCache(network, name, data);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GetHDWalletEndpoint), name.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(data).ToLower());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out WalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = AddAddressesToWalletEndpointNoCache(network, name, data);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-addresses-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public WalletEndpointResponse GetWalletAddressesEndpoint(BlockCypherNetwork network, string name, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GetWalletAddressesEndpointNoCache(network, name);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GetWalletAddressesEndpoint), name.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out WalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GetWalletAddressesEndpointNoCache(network, name);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// https://www.blockcypher.com/dev/bitcoin/?shell#get-wallet-addresses-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="name"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        public HDWalletEndpointResponse GetHDWalletAddressesEndpoint(BlockCypherNetwork network, string name, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10)
        {
            if (cacheMode == BlockCypherCacheMode.None)
                return GetHDWalletAddressesEndpointNoCache(network, name);
            else
            {
                string cacheKey = BlockCypherCacheProvider.GenerateCacheKey(nameof(GetHDWalletAddressesEndpoint), name.ToString());
                bool exists = BlockCypherCacheProvider.Get(cacheKey, out HDWalletEndpointResponse cacheData);
                if (!exists)
                {
                    cacheData = GetHDWalletAddressesEndpointNoCache(network, name);

                    BlockCypherCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(cacheSeconds));
                }

                return cacheData;
            }
        }
        #endregion
    }
}
