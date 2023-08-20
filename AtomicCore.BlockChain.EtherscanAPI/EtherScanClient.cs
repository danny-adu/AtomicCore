using System;
using System.Numerics;
using System.Text;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// IEtherScanClient interface implementation
    /// </summary>
    public class EtherScanClient : IEtherScanClient
    {
        #region Variables

        /// <summary>
        /// eth
        /// </summary>
        public const string c_eth_main = "https://api.etherscan.io";

        /// <summary>
        /// eth-cn代理
        /// </summary>
        public const string c_eth_cn = "https://api-cn.etherscan.com";

        /// <summary>
        /// ropsten
        /// </summary>
        public const string c_eth_ropsten = "https://api-ropsten.etherscan.io";

        /// <summary>
        /// kovan
        /// </summary>
        public const string c_eth_kovan = "https://api-kovan.etherscan.io";

        /// <summary>
        /// rinkeby
        /// </summary>
        public const string c_eth_rinkeby = "https://api-rinkeby.etherscan.io";

        /// <summary>
        /// goerli
        /// </summary>
        public const string c_eth_goerli = "https://api-goerli.etherscan.io";

        /// <summary>
        /// ApiKey Temp Append To End,eg => apikey={0}
        /// </summary>
        private const string c_apiKeyTemp = "&apikey={0}";

        /// <summary>
        /// 标记为最后一个TAG,查询余额等请求会用到该参数
        /// </summary>
        private const string c_latestTag = "&tag=latest";

        /// <summary>
        /// 追加地址
        /// </summary>
        private const string c_addressTemp = "&address={0}";

        /// <summary>
        /// 追加合约地址参数
        /// </summary>
        private const string c_contractAddressTemp = "&contractaddress={0}";

        /// <summary>
        /// API KEY
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// base url
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// agent url tmp
        /// </summary>
        private readonly string _agentGetTmp;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="apiKey">API-KEY</param>
        /// <param name="baseUrl">基础URL</param>
        /// <param name="agentGetTmp">代理模版</param>
        public EtherScanClient(string apiKey, string baseUrl = c_eth_cn, string agentGetTmp = null)
        {
            this._apiKey = apiKey;
            this._baseUrl = baseUrl;
            this._agentGetTmp = agentGetTmp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 创建Rest Url
        /// </summary>
        /// <param name="module">模块名称</param>
        /// <param name="action">行为名称</param>
        /// <returns></returns>
        private string CreateRestUrl(string module, string action)
        {
            return string.Format(
                "{0}/api?module={1}&action={2}{3}",
                this._baseUrl,
                module,
                action,
                string.IsNullOrEmpty(this._apiKey) ?
                    string.Empty :
                    string.Format(c_apiKeyTemp, this._apiKey)
            );
        }

        /// <summary>
        /// Rest Get Request
        /// </summary>
        /// <param name="url">请求URL</param>
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
        /// JSON解析OBJECT
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

        /// <summary>
        /// JSON解析单模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private EtherscanSingleResult<T> SingleParse<T>(string resp)
        {
            EtherscanSingleResult<T> jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<EtherscanSingleResult<T>>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        /// <summary>
        /// JSON解析列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private EtherscanListResult<T> ListParse<T>(string resp)
            where T : class, new()
        {
            EtherscanListResult<T> jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<EtherscanListResult<T>>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        /// <summary>
        /// 解析RPC代理返回
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private EtherscanProxyResult ParseRpcResponse(string resp)
        {
            EtherscanProxyResult jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<EtherscanProxyResult>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        #endregion

        #region IEtherScanClient Methods

        #region IEtherAccounts

        /// <summary>
        /// 获取地址余额(若数额超过decimal的最大值会抛出数据异常)
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">合约地址,若为空则表示为查询主链行为</param>
        /// <param name="contractDecimals">合约代码小数位</param>
        /// <returns></returns>
        public EtherscanSingleResult<decimal> GetBalance(string address, string contractAddress = null, int contractDecimals = 0)
        {
            //基础判断
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            //URL拼接
            StringBuilder urlBuilder;
            if (string.IsNullOrEmpty(contractAddress))
                urlBuilder = new StringBuilder(this.CreateRestUrl("account", "balance"));
            else
            {
                urlBuilder = new StringBuilder(this.CreateRestUrl("account", "tokenbalance"));
                urlBuilder.AppendFormat(c_contractAddressTemp, contractAddress);
            }
            urlBuilder.AppendFormat(c_addressTemp, address);
            urlBuilder.Append(c_latestTag);

            //请求API
            string url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanSingleResult<BigInteger> jsonResult = SingleParse<BigInteger>(resp);
            if (jsonResult.Status != EtherscanJsonStatus.Success)
            {
                return new EtherscanSingleResult<decimal>
                {
                    Status = jsonResult.Status,
                    Message = jsonResult.Message,
                    Url = url,
                    Result = decimal.Zero
                };
            }

            //定义返回值
            decimal balance;
            if (string.IsNullOrEmpty(contractAddress))
                balance = Nethereum.Util.UnitConversion.Convert.FromWei(jsonResult.Result);
            else
            {
                if (contractDecimals > 0)
                    balance = Nethereum.Util.UnitConversion.Convert.FromWei(jsonResult.Result, contractDecimals);
                else
                    balance = (decimal)jsonResult.Result;
            }

            return new EtherscanSingleResult<decimal>
            {
                Status = jsonResult.Status,
                Message = jsonResult.Message,
                Url = url,
                Result = balance
            };
        }

        /// <summary>
        /// 获取地址余额(真实最小小数位)
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">合约地址,若为空则表示为查询主链行为</param>
        /// <returns></returns>
        public EtherscanSingleResult<BigInteger> GetBalanceRaw(string address, string contractAddress = null)
        {
            //基础判断
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            //URL拼接
            StringBuilder urlBuilder;
            if (string.IsNullOrEmpty(contractAddress))
                urlBuilder = new StringBuilder(this.CreateRestUrl("account", "balance"));
            else
            {
                urlBuilder = new StringBuilder(this.CreateRestUrl("account", "tokenbalance"));
                urlBuilder.AppendFormat(c_contractAddressTemp, contractAddress);
            }
            urlBuilder.AppendFormat(c_addressTemp, address);
            urlBuilder.Append(c_latestTag);

            //请求API
            string url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanSingleResult<BigInteger> jsonResult = SingleParse<BigInteger>(resp);
            if (jsonResult.Status != EtherscanJsonStatus.Success)
            {
                return new EtherscanSingleResult<BigInteger>
                {
                    Status = jsonResult.Status,
                    Message = jsonResult.Message,
                    Url = url,
                    Result = BigInteger.Zero
                };
            }

            return new EtherscanSingleResult<BigInteger>
            {
                Status = jsonResult.Status,
                Message = jsonResult.Message,
                Url = url,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// 获取交易记录列表
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="startBlock">起始区块高度</param>
        /// <param name="endBlock">截止区块高度</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页多少条数据</param>
        /// <returns></returns>
        public EtherscanListResult<EthNormalTransactionJsonResult> GetNormalTransactions(string address, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000)
        {
            //拼接URL
            string url = this.CreateRestUrl("account", "txlist");

            //请求参数拼接
            StringBuilder urlBuilder = new StringBuilder(url);
            urlBuilder.AppendFormat("&address={0}", address);
            urlBuilder.AppendFormat("&sort={0}", sort.ToString());
            if (null != startBlock && startBlock > 0)
                urlBuilder.AppendFormat("&startblock={0}", startBlock);
            if (null != endBlock && endBlock > 0)
                urlBuilder.AppendFormat("&endblock={0}", endBlock);
            if (null != page && page > 0)
                urlBuilder.AppendFormat("&page={0}", page);
            if (null != limit && limit > 0)
                urlBuilder.AppendFormat("&offset={0}", limit);

            //请求API
            url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanListResult<EthNormalTransactionJsonResult> jsonResult = ListParse<EthNormalTransactionJsonResult>(resp);
            jsonResult.Url = url;

            return jsonResult;
        }

        /// <summary>
        /// 获取指定地址的内部交易
        /// </summary>
        /// <param name="address">指定地址的内部交易,一般为合约地址</param>
        /// <param name="startBlock">起始区块</param>
        /// <param name="endBlock">结束区块</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页容量</param>
        /// <returns></returns>
        public EtherscanListResult<EthInternalTransactionJsonResult> GetInternalTransactions(string address, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000)
        {
            //拼接URL
            string url = this.CreateRestUrl("account", "txlistinternal");

            //请求参数拼接
            StringBuilder urlBuilder = new StringBuilder(url);
            urlBuilder.AppendFormat("&address={0}", address);
            urlBuilder.AppendFormat("&sort={0}", sort.ToString());
            if (null != startBlock && startBlock > 0)
                urlBuilder.AppendFormat("&startblock={0}", startBlock);
            if (null != endBlock && endBlock > 0)
                urlBuilder.AppendFormat("&endblock={0}", endBlock);
            if (null != page && page > 0)
                urlBuilder.AppendFormat("&page={0}", page);
            if (null != limit && limit > 0)
                urlBuilder.AppendFormat("&offset={0}", limit);

            //请求API
            url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanListResult<EthInternalTransactionJsonResult> jsonResult = ListParse<EthInternalTransactionJsonResult>(resp);
            jsonResult.Url = url;

            return jsonResult;
        }

        /// <summary>
        /// 获取指定地址的ERC20交易
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contract">合约地址,若不传则查询所有与该地址有关系的合约交易</param>
        /// <param name="startBlock">起始区块</param>
        /// <param name="endBlock">结束区块</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页容量</param>
        /// <returns></returns>
        public EtherscanListResult<EthErc20TransactionJsonResult> GetERC20Transactions(string address, string contract = null, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000)
        {
            //拼接URL
            string url = this.CreateRestUrl("account", "tokentx");

            //请求参数拼接
            StringBuilder urlBuilder = new StringBuilder(url);
            urlBuilder.AppendFormat("&address={0}", address);
            urlBuilder.AppendFormat("&sort={0}", sort.ToString());
            if (null != startBlock && startBlock > 0)
                urlBuilder.AppendFormat("&startblock={0}", startBlock);
            if (null != endBlock && endBlock > 0)
                urlBuilder.AppendFormat("&endblock={0}", endBlock);
            if (null != page && page > 0)
                urlBuilder.AppendFormat("&page={0}", page);
            if (null != limit && limit > 0)
                urlBuilder.AppendFormat("&offset={0}", limit);
            if (!string.IsNullOrEmpty(contract))
                urlBuilder.AppendFormat("&contractaddress={0}", contract);

            //请求API
            url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanListResult<EthErc20TransactionJsonResult> jsonResult = ListParse<EthErc20TransactionJsonResult>(resp);
            jsonResult.Url = url;

            return jsonResult;
        }

        #endregion

        #region IEtherContracts

        /// <summary>
        /// 获取合约ABI接口
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public EtherscanSingleResult<string> GetContractAbi(string address)
        {
            //基础判断
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            //拼接URL
            StringBuilder urlBuilder = new StringBuilder(this.CreateRestUrl("contract", "getabi"));
            urlBuilder.AppendFormat(c_addressTemp, address);

            //请求API
            string url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanSingleResult<string> jsonResult = SingleParse<string>(resp);
            jsonResult.Url = url;

            return jsonResult;
        }

        #endregion

        #region IEtherProxy

        /// <summary>
        /// Returns the number of most recent block
        /// </summary>
        /// <returns></returns>
        public EtherscanSingleResult<long> GetBlockNumber()
        {
            //拼接URL
            StringBuilder urlBuilder = new StringBuilder(this.CreateRestUrl("proxy", "eth_blockNumber"));

            //请求API
            string url = urlBuilder.ToString();
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanProxyResult proxyResult = ParseRpcResponse(resp);

            return new EtherscanSingleResult<long>()
            {
                Status = EtherscanJsonStatus.Success,
                Message = string.Empty,
                Url = url,
                Result = Convert.ToInt64(proxyResult.Result, 16)
            };
        }

        /// <summary>
        /// Returns the number of transactions performed by an address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public EtherscanSingleResult<long> GetTransactionCount(string address)
        {
            //基础判断
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            //拼接URL
            StringBuilder urlBuilder = new StringBuilder(this.CreateRestUrl("proxy", "eth_getTransactionCount"));
            urlBuilder.AppendFormat(c_addressTemp, address);

            //请求API
            string url = urlBuilder.ToString();
            string resp = this.RestGet(urlBuilder.ToString());

            //解析JSON
            EtherscanProxyResult proxyResult = ParseRpcResponse(resp);

            return new EtherscanSingleResult<long>()
            {
                Status = EtherscanJsonStatus.Success,
                Message = string.Empty,
                Result = Convert.ToInt64(proxyResult.Result, 16),
                Url = url
            };
        }

        #endregion

        #region IEtherGasTracker

        /// <summary>
        /// 获取网络手续费（三档）
        /// </summary>
        /// <returns></returns>
        public EtherscanSingleResult<EthGasOracleJsonResult> GetGasOracle()
        {
            //拼接URL
            string url = this.CreateRestUrl("gastracker", "gasoracle");

            //请求API
            string resp = this.RestGet(url);

            //解析JSON
            EtherscanSingleResult<EthGasOracleJsonResult> jsonResult = SingleParse<EthGasOracleJsonResult>(resp);
            jsonResult.Url = url;

            return jsonResult;
        }

        #endregion

        #endregion
    }
}
