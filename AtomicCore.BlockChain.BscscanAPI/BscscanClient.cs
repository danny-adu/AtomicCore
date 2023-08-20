using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bscscan client service
    /// </summary>
    public class BscscanClient : IBscscanClient
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

        /// <summary>
        /// api key token 
        /// </summary>
        private string _apiKeyToken = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="agentGetTmp"></param>
        /// <param name="agentPostTmp"></param>
        public BscscanClient(string agentGetTmp = null, string agentPostTmp = null)
        {
            _agentGetTmp = agentGetTmp;
            _agentPostTmp = agentPostTmp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// get base url
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private string GetBaseUrl(BscNetwork network)
        {
            string baseUrl = network switch
            {
                BscNetwork.BscMainnet => "https://api.bscscan.com",
                BscNetwork.BscTestnet => "https://api-testnet.bscscan.com",
                _ => throw new NotImplementedException(),
            };

            return baseUrl;
        }

        /// <summary>
        /// create rest url
        /// </summary>
        /// <param name="network">version</param>
        /// <param name="module">module</param>
        /// <param name="action">action</param>
        /// <param name="data">query data</param>
        /// <returns></returns>
        private string GetRestUrl(BscNetwork network, BscModule module, string action, Dictionary<string, string> data = null)
        {
            string baseUrl = GetBaseUrl(network);

            if (null == data || data.Count <= 0)
                return $"{baseUrl}/api?module={module}&action={action}&apikey={_apiKeyToken}".ToLower();
            else
                return $"{baseUrl}/api?module={module}&action={action}&apikey={_apiKeyToken}&{string.Join("&", data.Select(s => $"{s.Key}={s.Value}"))}".ToLower();
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
        /// json -> check error propertys
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private string GetErrorMsg(string resp)
        {
            BscscanMsgResult jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BscscanMsgResult>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (jsonResult.Status == BscscanJsonStatus.Success)
                return null;

            BscscanSingleResult<string> errorJson;
            try
            {
                errorJson = Newtonsoft.Json.JsonConvert.DeserializeObject<BscscanSingleResult<string>>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return $"{jsonResult.Message} --> {errorJson.Result}";
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

        #region No Cache Methods

        #region Accounts

        /// <summary>
        /// Get Balance
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<decimal> GetBalance(string address, BscBlockTag tag, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "balance", new Dictionary<string, string>()
            {
                { "address",address },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<long> jsonResult = ObjectParse<BscscanSingleResult<long>>(resp);

            if (jsonResult.Status == BscscanJsonStatus.Success)
            {
                return new BscscanSingleResult<decimal>()
                {
                    Status = BscscanJsonStatus.Success,
                    Message = jsonResult.Message,
                    Result = UnitConversion.Convert.FromWei(jsonResult.Result, UnitConversion.EthUnit.Ether)
                };
            }
            else
                return new BscscanSingleResult<decimal>(BscscanJsonStatus.Failure, jsonResult.Message);
        }

        /// <summary>
        /// Get Balance
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BigInteger> GetBalanceRaw(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "balance", new Dictionary<string, string>()
            {
                { "address",address },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<BigInteger> jsonResult = ObjectParse<BscscanSingleResult<BigInteger>>(resp);

            if (jsonResult.Status == BscscanJsonStatus.Success)
            {
                return new BscscanSingleResult<BigInteger>()
                {
                    Status = BscscanJsonStatus.Success,
                    Message = jsonResult.Message,
                    Result = jsonResult.Result
                };
            }
            else
                return new BscscanSingleResult<BigInteger>(BscscanJsonStatus.Failure, jsonResult.Message);
        }

        /// <summary>
        /// Get Balance List
        /// </summary>
        /// <param name="address">the string representing the addressies to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanListResult<BscAccountBalanceJson> GetBalanceList(string[] address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "balancemulti", new Dictionary<string, string>()
            {
                { "address",string.Join(",",address) },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscAccountBalanceJson> jsonResult = ObjectParse<BscscanListResult<BscAccountBalanceJson>>(resp);

            return jsonResult;
        }

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
        /// <returns></returns>
        private BscscanListResult<BscNormalTransactionJson> GetNormalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "txlist", new Dictionary<string, string>()
            {
                { "address",address },
                { "startblock",startblock.ToString() },
                { "endblock",endblock.ToString() },
                { "page",page.ToString() },
                { "offset",offset.ToString() },
                { "sort",sort.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscNormalTransactionJson> jsonResult = ObjectParse<BscscanListResult<BscNormalTransactionJson>>(resp);

            return jsonResult;
        }

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
        /// <returns></returns>
        private BscscanListResult<BscInternalTransactionJson> GetInternalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "txlistinternal", new Dictionary<string, string>()
            {
                { "address",address },
                { "startblock",startblock.ToString() },
                { "endblock",endblock.ToString() },
                { "page",page.ToString() },
                { "offset",offset.ToString() },
                { "sort",sort.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscInternalTransactionJson> jsonResult = ObjectParse<BscscanListResult<BscInternalTransactionJson>>(resp);

            return jsonResult;
        }

        /// <summary>
        /// Returns the list of internal transactions performed within a transaction.
        ///     Note : This API endpoint returns a maximum of 10000 records only.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check for internal transactions</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanListResult<BscInternalEventJson> GetInternalTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "txlistinternal", new Dictionary<string, string>()
            {
                { "txhash",txhash }
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscInternalEventJson> jsonResult = ObjectParse<BscscanListResult<BscInternalEventJson>>(resp);

            return jsonResult;
        }

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
        /// <returns></returns>
        private BscscanListResult<BscBEP20TransactionJson> GetBEP20TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet)
        {
            //basic params
            var jsonParams = new Dictionary<string, string>()
            {
                { "address",address },
                { "startblock",startblock.ToString() },
                { "endblock",endblock.ToString() },
                { "page",page.ToString() },
                { "offset",offset.ToString() },
                { "sort",sort.ToString().ToLower() }
            };
            if (!string.IsNullOrEmpty(contractaddress))
                jsonParams.Add("contractaddress", contractaddress);

            string url = this.GetRestUrl(network, BscModule.Account, "tokentx", jsonParams);

            string resp = this.RestGet(url);
            BscscanListResult<BscBEP20TransactionJson> jsonResult = ObjectParse<BscscanListResult<BscBEP20TransactionJson>>(resp);
            if (jsonResult.Status == BscscanJsonStatus.Success)
                jsonResult.Result.ForEach(e =>
                {
                    e.TxValue = UnitConversion.Convert.FromWei(new BigInteger(e.TxValue), e.TokenDecimal);
                });

            return jsonResult;
        }

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
        /// <returns></returns>
        private BscscanListResult<BscBEP721TransactionJson> GetBEP721TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet)
        {
            //basic params
            var jsonParams = new Dictionary<string, string>()
            {
                { "address",address },
                { "startblock",startblock.ToString() },
                { "endblock",endblock.ToString() },
                { "page",page.ToString() },
                { "offset",offset.ToString() },
                { "sort",sort.ToString().ToLower() }
            };
            if (!string.IsNullOrEmpty(contractaddress))
                jsonParams.Add("contractaddress", contractaddress);

            string url = this.GetRestUrl(network, BscModule.Account, "tokennfttx", jsonParams);

            string resp = this.RestGet(url);
            BscscanListResult<BscBEP721TransactionJson> jsonResult = ObjectParse<BscscanListResult<BscBEP721TransactionJson>>(resp);

            return jsonResult;
        }

        /// <summary>
        /// Returns the list of blocks validated by an address.
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="blocktype">the string pre-defined block type, blocksfor canonical blocks </param>
        /// <param name="page">the integer page number, if pagination is enabled</param>
        /// <param name="offset">the number of transactions displayed per page</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanListResult<BscMineRewardJson> GetMinedBlockListByAddress(string address, string blocktype, int page = 1, int offset = 10000, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "getminedblocks", new Dictionary<string, string>()
            {
                { "address",address },
                { "blocktype",blocktype },
                { "page",page.ToString() },
                { "offset",offset.ToString() },
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscMineRewardJson> jsonResult = ObjectParse<BscscanListResult<BscMineRewardJson>>(resp);

            return jsonResult;
        }

        #endregion

        #region Contracts

        /// <summary>
        /// Returns the contract Application Binary Interface ( ABI ) of a verified smart contract.
        /// </summary>
        /// <param name="contractAddress">the contract address that has a verified source code</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<string> GetContractABI(string contractAddress, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Contract, "getabi", new Dictionary<string, string>()
            {
                { "address",contractAddress }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<string> jsonResult = ObjectParse<BscscanSingleResult<string>>(resp);

            return jsonResult;
        }

        #endregion

        #region Transactions

        /// <summary>
        /// Returns the status code of a transaction execution.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check the execution status</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscTransactionReceiptStatusJson> GetTransactionReceiptStatus(string txhash, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Transaction, "gettxreceiptstatus", new Dictionary<string, string>()
            {
                { "txhash",txhash }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<BscTransactionReceiptStatusJson> jsonResult = ObjectParse<BscscanSingleResult<BscTransactionReceiptStatusJson>>(resp);

            return jsonResult;
        }

        #endregion

        #region Blocks

        /// <summary>
        /// Returns the block reward awarded for validating a certain block. 
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscBlockRewardJson> GetBlockRewardByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Block, "getblockreward", new Dictionary<string, string>()
            {
                { "blockno",blockNo.ToString() }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<BscBlockRewardJson> jsonResult = ObjectParse<BscscanSingleResult<BscBlockRewardJson>>(resp);

            return jsonResult;
        }

        /// <summary>
        /// Returns the estimated time remaining, in seconds, until a certain block is validated.
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscBlockEstimatedJson> GetBlockEstimatedByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Block, "getblockcountdown", new Dictionary<string, string>()
            {
                { "blockno",blockNo.ToString() }
            });

            string resp = this.RestGet(url);
            string errorMsg = GetErrorMsg(resp);
            if (string.IsNullOrEmpty(errorMsg))
            {
                BscscanSingleResult<BscBlockEstimatedJson> jsonResult = ObjectParse<BscscanSingleResult<BscBlockEstimatedJson>>(resp);

                return jsonResult;
            }
            else
                return new BscscanSingleResult<BscBlockEstimatedJson>(BscscanJsonStatus.Failure, errorMsg);
        }

        /// <summary>
        /// Returns the block number that was validated at a certain timestamp.
        /// Tip : 
        ///         Convert a regular date-time to a Unix timestamp.
        /// </summary>
        /// <param name="timestamp">the integer representing the Unix timestamp in seconds.</param>
        /// <param name="closest">the closest available block to the provided timestamp, either before or after</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<long> GetBlockNumberByTimestamp(long timestamp, BscClosest closest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Block, "getblocknobytime", new Dictionary<string, string>()
            {
                { "timestamp",timestamp.ToString() },
                { "closest",closest.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanSingleResult<long> jsonResult = ObjectParse<BscscanSingleResult<long>>(resp);

            return jsonResult;
        }

        /// <summary>
        /// Returns the daily average block size within a date range.
        /// </summary>
        /// <param name="startdate">the starting date in yyyy-MM-dd format, eg. 2021-08-01</param>
        /// <param name="enddate">the ending date in yyyy-MM-dd format, eg. 2021-08-31</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanListResult<BscBlockAvgSizeJson> GetDailyAverageBlockSize(DateTime startdate, DateTime enddate, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Block, "dailyavgblocksize", new Dictionary<string, string>()
            {
                { "startdate",startdate.ToString("yyyy-MM-dd") },
                { "enddate",enddate.ToString("yyyy-MM-dd") },
                { "sort",sort.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscscanListResult<BscBlockAvgSizeJson> jsonResult = ObjectParse<BscscanListResult<BscBlockAvgSizeJson>>(resp);

            return jsonResult;
        }

        #endregion

        #region Geth Proxy

        /// <summary>
        /// Returns the number of most recent block
        /// </summary>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<long> GetBlockNumber(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_blockNumber");

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<long>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = (long)new HexBigInteger(jsonResult.Result).Value
            };
        }

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscRpcBlockSimpleJson> GetBlockSimpleByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getBlockByNumber", new Dictionary<string, string>()
            {
                { "tag",new HexBigInteger(blockNumber).HexValue },
                { "boolean","false" },
            });

            string resp = this.RestGet(url);
            BscRpcJson<BscRpcBlockSimpleJson> jsonResult = ObjectParse<BscRpcJson<BscRpcBlockSimpleJson>>(resp);

            return new BscscanSingleResult<BscRpcBlockSimpleJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscRpcBlockFullJson> GetBlockFullByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getBlockByNumber", new Dictionary<string, string>()
            {
                { "tag",new HexBigInteger(blockNumber).HexValue },
                { "boolean","true" },
            });

            string resp = this.RestGet(url);
            BscRpcJson<BscRpcBlockFullJson> jsonResult = ObjectParse<BscRpcJson<BscRpcBlockFullJson>>(resp);

            return new BscscanSingleResult<BscRpcBlockFullJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the number of transactions in a block.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<int> GetBlockTransactionCountByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getBlockTransactionCountByNumber", new Dictionary<string, string>()
            {
                { "tag",new HexBigInteger(blockNumber).HexValue }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<int>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = Convert.ToInt32(jsonResult.Result, 16)
            };
        }

        /// <summary>
        /// Returns information about a transaction requested by transaction hash.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscRpcTransactionJson> GetTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getTransactionByHash", new Dictionary<string, string>()
            {
                { "txhash",txhash }
            });

            string resp = this.RestGet(url);
            BscRpcJson<BscRpcTransactionJson> jsonResult = ObjectParse<BscRpcJson<BscRpcTransactionJson>>(resp);

            return new BscscanSingleResult<BscRpcTransactionJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns information about a transaction by block number and transaction index position.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="index">the position of the uncle's index in the block, in hex eg. 0x1</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscRpcTransactionJson> GetTransactionByBlockNumberAndIndex(long blockNumber, int index, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getTransactionByBlockNumberAndIndex", new Dictionary<string, string>()
            {
                { "tag",new HexBigInteger(blockNumber).HexValue },
                { "index",$"0x{index:X}" }
            });

            string resp = this.RestGet(url);
            BscRpcJson<BscRpcTransactionJson> jsonResult = ObjectParse<BscRpcJson<BscRpcTransactionJson>>(resp);

            return new BscscanSingleResult<BscRpcTransactionJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the number of transactions performed by an address.
        /// </summary>
        /// <param name="address">the string representing the address to get transaction count</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<int> GetTransactionCount(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getTransactionCount", new Dictionary<string, string>()
            {
                { "address",address },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<int>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = Convert.ToInt32(jsonResult.Result, 16)
            };
        }

        /// <summary>
        /// Returns the receipt of a transaction that has been validated.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscRpcTransactionReceiptJson> GetTransactionReceipt(string txhash, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getTransactionReceipt", new Dictionary<string, string>()
            {
                { "txhash",txhash }
            });

            string resp = this.RestGet(url);
            BscRpcJson<BscRpcTransactionReceiptJson> jsonResult = ObjectParse<BscRpcJson<BscRpcTransactionReceiptJson>>(resp);

            return new BscscanSingleResult<BscRpcTransactionReceiptJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Executes a new message call immediately without creating a transaction on the block chain.
        /// </summary>
        /// <param name="to">the string representing the address to interact with</param>
        /// <param name="data">the hash of the method signature and encoded parameters</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<string> Call(string to, string data, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_call", new Dictionary<string, string>()
            {
                { "to",to },
                { "data",data },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<string>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns code at a given address.
        /// </summary>
        /// <param name="address">the string representing the address to get code</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<string> GetCode(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getCode", new Dictionary<string, string>()
            {
                { "address",address },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<string>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the value from a storage position at a given address.
        /// </summary>
        /// <param name="address">the string representing the address to get code</param>
        /// <param name="position">the hex code of the position in storage, eg 0x0</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<string> GetStorageAt(string address, string position, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_getStorageAt", new Dictionary<string, string>()
            {
                { "address",address },
                { "position",position },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<string>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the current price per gas in wei.
        /// </summary>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<long> GasPrice(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_gasPrice");

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<long>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = Convert.ToInt64(jsonResult.Result, 16)
            };
        }

        /// <summary>
        /// Makes a call or transaction, which won't be added to the blockchain and returns the gas used. 
        /// </summary>
        /// <param name="data">the hash of the method signature and encoded parameters</param>
        /// <param name="to">the string representing the address to interact with</param>
        /// <param name="value">the value sent in this transaction, in hex eg. 0xff22</param>
        /// <param name="gasPrice">the gas price paid for each unit of gas, in wei</param>
        /// <param name="gas">the amount of gas provided for the transaction, in hex eg. 0x5f5e0ff</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<long> EstimateGas(string data, string to, string value, string gasPrice, string gas, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_estimateGas", new Dictionary<string, string>()
            {
                { "data",data },
                { "to",to },
                { "value",value },
                { "gas",gas },
                { "gasPrice",gasPrice }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<long>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = Convert.ToInt64(jsonResult.Result, 16)
            };
        }

        #endregion

        #region Tokens

        /// <summary>
        /// Returns the total supply of a BEP-20 token.
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BigInteger> GetBEP20TotalSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Stats, "tokensupply", new Dictionary<string, string>()
            {
                { "contractaddress",contractaddress }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<BigInteger>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = BigInteger.Parse(jsonResult.Result)
            };
        }

        /// <summary>
        /// Returns the current circulating supply of a BEP-20 token. 
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BigInteger> GetBEP20CirculatingSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Stats, "tokenCsupply", new Dictionary<string, string>()
            {
                { "contractaddress",contractaddress }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<BigInteger>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = BigInteger.Parse(jsonResult.Result)
            };
        }

        /// <summary>
        /// Returns the current balance of a BEP-20 token of an address.
        /// </summary>
        /// <param name="address">the string representing the address to check for token balance</param>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BigInteger> GetBEP20BalanceOf(string address, string contractaddress, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Account, "tokenbalance", new Dictionary<string, string>()
            {
                { "address",address },
                { "contractaddress",contractaddress },
                { "tag",tag.ToString().ToLower() }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<BigInteger>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = BigInteger.Parse(jsonResult.Result)
            };
        }

        #endregion

        #region Gas Tracker

        /// <summary>
        /// Returns the current Safe, Proposed and Fast gas prices. 
        /// </summary>
        /// <param name="network">network</param>
        /// <returns></returns>
        private BscscanSingleResult<BscGasOracleJson> GetGasOracle(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.GasTracker, "gasoracle");

            string resp = this.RestGet(url);
            BscscanSingleResult<BscGasOracleJson> jsonResult = ObjectParse<BscscanSingleResult<BscGasOracleJson>>(resp);

            return jsonResult;
        }

        #endregion

        #region Stats

        /// <summary>
        /// Returns the current amount of BNB in circulation.
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private BscscanSingleResult<BigInteger> GetBNBTotalSupply(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Stats, "bnbsupply");

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<BigInteger>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = BigInteger.Parse(jsonResult.Result)
            };
        }

        /// <summary>
        /// Returns the top 21 validators for the Binance Smart Chain.
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private BscscanListResult<BscValidatorJson> GetBscValidatorList(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Stats, "validators");

            string resp = this.RestGet(url);
            BscRpcJson<List<BscValidatorJson>> jsonResult = ObjectParse<BscRpcJson<List<BscValidatorJson>>>(resp);

            return new BscscanListResult<BscValidatorJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the latest price of 1 BNB.
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private BscscanSingleResult<BscLastPriceJson> GetBNBLastPrice(BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Stats, "bnbprice");

            string resp = this.RestGet(url);
            BscRpcJson<BscLastPriceJson> jsonResult = ObjectParse<BscRpcJson<BscLastPriceJson>>(resp);

            return new BscscanSingleResult<BscLastPriceJson>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        #endregion

        #endregion

        #region IBscscanClient

        #region SetApiKeyToken

        /// <summary>
        /// set api key token
        /// </summary>
        /// <param name="apiKeyToken"></param>
        public void SetApiKeyToken(string apiKeyToken)
        {
            _apiKeyToken = apiKeyToken;
        }

        #endregion

        #region IBscAccounts

        /// <summary>
        /// Get Balance
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<decimal> GetBalance(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBalance(address, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBalance),
                    address,
                    tag.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<decimal> cacheData);
                if (!exists)
                {
                    cacheData = GetBalance(address, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Get Balance Raw
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BigInteger> GetBalanceRaw(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBalanceRaw(address, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBalanceRaw),
                    address,
                    tag.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BigInteger> cacheData);
                if (!exists)
                {
                    cacheData = GetBalanceRaw(address, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Get Balance List
        /// </summary>
        /// <param name="address">the string representing the address to check for balance</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanListResult<BscAccountBalanceJson> GetBalanceList(string[] address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBalanceList(address, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBalanceList),
                    string.Join(",", address.OrderBy(d => d)),
                    tag.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscAccountBalanceJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBalanceList(address, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanListResult<BscNormalTransactionJson> GetNormalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetNormalTransactionByAddress(address, startblock, endblock, page, offset, sort, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetNormalTransactionByAddress),
                    address,
                    startblock.ToString(),
                    endblock.ToString(),
                    page.ToString(),
                    offset.ToString(),
                    sort.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscNormalTransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetNormalTransactionByAddress(address, startblock, endblock, page, offset, sort, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanListResult<BscInternalTransactionJson> GetInternalTransactionByAddress(string address, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetInternalTransactionByAddress(address, startblock, endblock, page, offset, sort, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetInternalTransactionByAddress),
                    address,
                    startblock.ToString(),
                    endblock.ToString(),
                    page.ToString(),
                    offset.ToString(),
                    sort.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscInternalTransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetInternalTransactionByAddress(address, startblock, endblock, page, offset, sort, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the list of internal transactions performed within a transaction.
        ///     Note : This API endpoint returns a maximum of 10000 records only.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check for internal transactions</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanListResult<BscInternalEventJson> GetInternalTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetInternalTransactionByHash(txhash, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetInternalTransactionByHash),
                    txhash.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscInternalEventJson> cacheData);
                if (!exists)
                {
                    cacheData = GetInternalTransactionByHash(txhash, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanListResult<BscBEP20TransactionJson> GetBEP20TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBEP20TransactionByAddress(address, contractaddress, startblock, endblock, page, offset, sort, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBEP20TransactionByAddress),
                    address,
                    contractaddress,
                    startblock.ToString(),
                    endblock.ToString(),
                    page.ToString(),
                    offset.ToString(),
                    sort.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscBEP20TransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBEP20TransactionByAddress(address, contractaddress, startblock, endblock, page, offset, sort, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanListResult<BscBEP721TransactionJson> GetBEP721TransactionByAddress(string address, string contractaddress, int startblock = 0, int endblock = int.MaxValue, int page = 1, int offset = 10000, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBEP721TransactionByAddress(address, contractaddress, startblock, endblock, page, offset, sort, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBEP721TransactionByAddress),
                    address,
                    contractaddress,
                    startblock.ToString(),
                    endblock.ToString(),
                    page.ToString(),
                    offset.ToString(),
                    sort.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscBEP721TransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBEP721TransactionByAddress(address, contractaddress, startblock, endblock, page, offset, sort, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanListResult<BscMineRewardJson> GetMinedBlockListByAddress(string address, string blocktype, int page = 1, int offset = 10000, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetMinedBlockListByAddress(address, blocktype, page, offset, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetMinedBlockListByAddress),
                    address,
                    blocktype,
                    page.ToString(),
                    offset.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscMineRewardJson> cacheData);
                if (!exists)
                {
                    cacheData = GetMinedBlockListByAddress(address, blocktype, page, offset, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscContracts

        /// <summary>
        /// Returns the contract Application Binary Interface ( ABI ) of a verified smart contract.
        /// </summary>
        /// <param name="contractAddress">the contract address that has a verified source code</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<string> GetContractABI(string contractAddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetContractABI(contractAddress, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetContractABI),
                    contractAddress,
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<string> cacheData);
                if (!exists)
                {
                    cacheData = GetContractABI(contractAddress, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscTransactions

        /// <summary>
        /// Returns the status code of a transaction execution.
        /// </summary>
        /// <param name="txhash">the string representing the transaction hash to check the execution status</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscTransactionReceiptStatusJson> GetTransactionReceiptStatus(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetTransactionReceiptStatus(txhash, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetTransactionReceiptStatus),
                    txhash,
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscTransactionReceiptStatusJson> cacheData);
                if (!exists)
                {
                    cacheData = GetTransactionReceiptStatus(txhash, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscBlocks

        /// <summary>
        /// Returns the block reward awarded for validating a certain block. 
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscBlockRewardJson> GetBlockRewardByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockRewardByNumber(blockNo, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockRewardByNumber),
                    blockNo.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscBlockRewardJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockRewardByNumber(blockNo, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the estimated time remaining, in seconds, until a certain block is validated.
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscBlockEstimatedJson> GetBlockEstimatedByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockEstimatedByNumber(blockNo, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockEstimatedByNumber),
                    blockNo.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscBlockEstimatedJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockEstimatedByNumber(blockNo, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the block number that was validated at a certain timestamp.
        /// Tip : 
        ///         Convert a regular date-time to a Unix timestamp.
        /// </summary>
        /// <param name="timestamp">the integer representing the Unix timestamp in seconds.</param>
        /// <param name="closest">the closest available block to the provided timestamp, either before or after</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<long> GetBlockNumberByTimestamp(long timestamp, BscClosest closest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockNumberByTimestamp(timestamp, closest, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockNumberByTimestamp),
                    timestamp.ToString(),
                    closest.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<long> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockNumberByTimestamp(timestamp, closest, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the daily average block size within a date range.
        /// </summary>
        /// <param name="startdate">the starting date in yyyy-MM-dd format, eg. 2021-08-01</param>
        /// <param name="enddate">the ending date in yyyy-MM-dd format, eg. 2021-08-31</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanListResult<BscBlockAvgSizeJson> GetDailyAverageBlockSize(DateTime startdate, DateTime enddate, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetDailyAverageBlockSize(startdate, enddate, sort, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetDailyAverageBlockSize),
                    startdate.ToString("yyyy-MM-dd"),
                    enddate.ToString("yyyy-MM-dd"),
                    sort.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscBlockAvgSizeJson> cacheData);
                if (!exists)
                {
                    cacheData = GetDailyAverageBlockSize(startdate, enddate, sort, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscGethProxy

        /// <summary>
        /// Returns the number of most recent block
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<long> GetBlockNumber(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockNumber(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockNumber),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<long> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockNumber(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscRpcBlockSimpleJson> GetBlockSimpleByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockSimpleByNumber(blockNumber, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockSimpleByNumber),
                    blockNumber.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscRpcBlockSimpleJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockSimpleByNumber(blockNumber, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns information about a block by block number.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscRpcBlockFullJson> GetBlockFullByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockFullByNumber(blockNumber, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockFullByNumber),
                    blockNumber.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscRpcBlockFullJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockFullByNumber(blockNumber, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the number of transactions in a block.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<int> GetBlockTransactionCountByNumber(long blockNumber, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBlockTransactionCountByNumber(blockNumber, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBlockTransactionCountByNumber),
                    blockNumber.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<int> cacheData);
                if (!exists)
                {
                    cacheData = GetBlockTransactionCountByNumber(blockNumber, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns information about a transaction requested by transaction hash.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscRpcTransactionJson> GetTransactionByHash(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetTransactionByHash(txhash, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetTransactionByHash),
                    txhash.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscRpcTransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetTransactionByHash(txhash, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns information about a transaction by block number and transaction index position.
        /// </summary>
        /// <param name="blockNumber">the block number</param>
        /// <param name="index">the position of the uncle's index in the block, in hex eg. 0x1</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscRpcTransactionJson> GetTransactionByBlockNumberAndIndex(long blockNumber, int index, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetTransactionByBlockNumberAndIndex(blockNumber, index, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetTransactionByBlockNumberAndIndex),
                    blockNumber.ToString(),
                    index.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscRpcTransactionJson> cacheData);
                if (!exists)
                {
                    cacheData = GetTransactionByBlockNumberAndIndex(blockNumber, index, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the number of transactions performed by an address.
        /// </summary>
        /// <param name="address">the string representing the address to get transaction count</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<int> GetTransactionCount(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetTransactionCount(address, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetTransactionCount),
                    address.ToString(),
                    tag.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<int> cacheData);
                if (!exists)
                {
                    cacheData = GetTransactionCount(address, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Submits a pre-signed transaction for broadcast to the Binance Smart Chain network.
        /// </summary>
        /// <param name="hex">the string representing the signed raw transaction data to broadcast.</param>
        /// <param name="network">network</param>
        /// <returns></returns>
        public BscscanSingleResult<string> SendRawTransaction(string hex, BscNetwork network = BscNetwork.BscMainnet)
        {
            string url = this.GetRestUrl(network, BscModule.Proxy, "eth_sendRawTransaction", new Dictionary<string, string>()
            {
                { "hex",hex }
            });

            string resp = this.RestGet(url);
            BscRpcJson<string> jsonResult = ObjectParse<BscRpcJson<string>>(resp);

            return new BscscanSingleResult<string>()
            {
                Status = BscscanJsonStatus.Success,
                Message = string.Empty,
                Result = jsonResult.Result
            };
        }

        /// <summary>
        /// Returns the receipt of a transaction that has been validated.
        /// </summary>
        /// <param name="txhash">the string representing the hash of the transaction</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscRpcTransactionReceiptJson> GetTransactionReceipt(string txhash, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetTransactionReceipt(txhash, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetTransactionReceipt),
                    txhash.ToString(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscRpcTransactionReceiptJson> cacheData);
                if (!exists)
                {
                    cacheData = GetTransactionReceipt(txhash, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanSingleResult<string> Call(string to, string data, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return Call(to, data, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(Call),
                    to,
                    data,
                    tag.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<string> cacheData);
                if (!exists)
                {
                    cacheData = Call(to, data, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns code at a given address.
        /// </summary>
        /// <param name="address">the string representing the address to get code</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<string> GetCode(string address, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetCode(address, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetCode),
                    address,
                    tag.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<string> cacheData);
                if (!exists)
                {
                    cacheData = GetCode(address, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanSingleResult<string> GetStorageAt(string address, string position, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetStorageAt(address, position, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetStorageAt),
                    address,
                    tag.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<string> cacheData);
                if (!exists)
                {
                    cacheData = GetStorageAt(address, position, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the current price per gas in wei.
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<long> GasPrice(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GasPrice(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GasPrice),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<long> cacheData);
                if (!exists)
                {
                    cacheData = GasPrice(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

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
        public BscscanSingleResult<long> EstimateGas(string data, string to, string value, string gasPrice, string gas, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return EstimateGas(data, to, value, gasPrice, gas, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(EstimateGas),
                    data,
                    to,
                    value,
                    gasPrice,
                    gas,
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<long> cacheData);
                if (!exists)
                {
                    cacheData = EstimateGas(data, to, value, gasPrice, gas, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscTokens

        /// <summary>
        /// Returns the total supply of a BEP-20 token.
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BigInteger> GetBEP20TotalSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBEP20TotalSupply(contractaddress, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBEP20TotalSupply),
                    contractaddress,
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BigInteger> cacheData);
                if (!exists)
                {
                    cacheData = GetBEP20TotalSupply(contractaddress, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the current circulating supply of a BEP-20 token. 
        /// </summary>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BigInteger> GetBEP20CirculatingSupply(string contractaddress, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBEP20CirculatingSupply(contractaddress, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBEP20CirculatingSupply),
                    contractaddress,
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BigInteger> cacheData);
                if (!exists)
                {
                    cacheData = GetBEP20CirculatingSupply(contractaddress, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the current balance of a BEP-20 token of an address.
        /// </summary>
        /// <param name="address">the string representing the address to check for token balance</param>
        /// <param name="contractaddress">the contract address of the BEP-20 token</param>
        /// <param name="tag">the string pre-defined block parameter, either earliest, pending or latest</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BigInteger> GetBEP20BalanceOf(string address, string contractaddress, BscBlockTag tag = BscBlockTag.Latest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBEP20BalanceOf(address, contractaddress, tag, network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBEP20BalanceOf),
                    address,
                    contractaddress,
                    tag.ToString().ToLower(),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BigInteger> cacheData);
                if (!exists)
                {
                    cacheData = GetBEP20BalanceOf(address, contractaddress, tag, network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscGasTracker

        /// <summary>
        /// Returns the current Safe, Proposed and Fast gas prices. 
        /// </summary>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        public BscscanSingleResult<BscGasOracleJson> GetGasOracle(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetGasOracle(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(nameof(GetGasOracle), network.ToString());
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscGasOracleJson> cacheData);
                if (!exists)
                {
                    cacheData = GetGasOracle(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #region IBscStats

        /// <summary>
        /// Returns the current amount of BNB in circulation.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="expiredSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BscscanSingleResult<BigInteger> GetBNBTotalSupply(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBNBTotalSupply(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBNBTotalSupply),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BigInteger> cacheData);
                if (!exists)
                {
                    cacheData = GetBNBTotalSupply(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the top 21 validators for the Binance Smart Chain.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="expiredSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BscscanListResult<BscValidatorJson> GetBscValidatorList(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBscValidatorList(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBscValidatorList),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanListResult<BscValidatorJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBscValidatorList(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        /// <summary>
        /// Returns the latest price of 1 BNB.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="expiredSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BscscanSingleResult<BscLastPriceJson> GetBNBLastPrice(BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10)
        {
            if (cacheMode == BscscanCacheMode.None)
                return GetBNBLastPrice(network);
            else
            {
                string cacheKey = BscscanCacheProvider.GenerateCacheKey(
                    nameof(GetBNBLastPrice),
                    network.ToString()
                );
                bool exists = BscscanCacheProvider.Get(cacheKey, out BscscanSingleResult<BscLastPriceJson> cacheData);
                if (!exists)
                {
                    cacheData = GetBNBLastPrice(network);
                    BscscanCacheProvider.Set(cacheKey, cacheData, cacheMode, TimeSpan.FromSeconds(expiredSeconds));
                }

                return cacheData;
            }
        }

        #endregion

        #endregion
    }
}
