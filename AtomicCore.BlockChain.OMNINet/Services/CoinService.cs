// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 数字货币服务分布类
    /// </summary>
    /// <remarks>
    /// Implementation of API calls list, 
    /// as found at: https://en.bitcoin.it/wiki/Original_Bitcoin_client/API_Calls_list 
    /// (note: this list is often out-of-date so call "help" in your bitcoin-cli 
    /// to get the latest signatures)
    /// </remarks>
    public class CoinService : ICoinService
    {
        #region Variables

        protected readonly IRpcConnector _rpcConnector;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CoinService()
        {
            _rpcConnector = new RpcConnector(this);
            Parameters = new CoinParameters(this, null, null, null, null, 0);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useTestnet"></param>
        public CoinService(bool useTestnet)
            : this()
        {
            Parameters.UseTestnet = useTestnet;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="daemonUrl"></param>
        /// <param name="rpcUsername"></param>
        /// <param name="rpcPassword"></param>
        /// <param name="walletPassword"></param>
        public CoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword)
        {
            _rpcConnector = new RpcConnector(this);
            Parameters = new CoinParameters(this, daemonUrl, rpcUsername, rpcPassword, walletPassword, 0);
        }

        /// <summary>
        /// Constructor
        /// this provides support for cases where *.config files are not an option
        /// </summary>
        /// <param name="daemonUrl"></param>
        /// <param name="rpcUsername"></param>
        /// <param name="rpcPassword"></param>
        /// <param name="walletPassword"></param>
        /// <param name="rpcRequestTimeoutInSeconds"></param>
        public CoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword, short rpcRequestTimeoutInSeconds)
        {
            _rpcConnector = new RpcConnector(this);
            Parameters = new CoinParameters(this, daemonUrl, rpcUsername, rpcPassword, walletPassword, rpcRequestTimeoutInSeconds);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// parameters ready only
        /// </summary>
        public CoinParameters Parameters { get; private set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// get coin long name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Parameters.CoinLongName;
        }

        #endregion

        #region Rpc Service

        public string AddMultiSigAddress(int nRquired, List<string> publicKeys, string account)
        {
            return account != null
                       ? _rpcConnector.MakeRequest<string>(RpcMethods.addmultisigaddress, nRquired, publicKeys, account)
                       : _rpcConnector.MakeRequest<string>(RpcMethods.addmultisigaddress, nRquired, publicKeys);
        }

        public string AddNode(string node, NodeAction action)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.addnode, node, action.ToString());
        }

        public string BackupWallet(string destination)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.backupwallet, destination);
        }

        public CreateMultiSigResponse CreateMultiSig(int nRquired, List<string> publicKeys)
        {
            return _rpcConnector.MakeRequest<CreateMultiSigResponse>(RpcMethods.createmultisig, nRquired, publicKeys);
        }

        public string CreateRawTransaction(CreateRawTransactionRequest rawTransaction)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.createrawtransaction, rawTransaction.Inputs, rawTransaction.Outputs);
        }

        public DecodeRawTransactionResponse DecodeRawTransaction(string rawTransactionHexString)
        {
            return _rpcConnector.MakeRequest<DecodeRawTransactionResponse>(RpcMethods.decoderawtransaction, rawTransactionHexString);
        }

        public DecodeScriptResponse DecodeScript(string hexString)
        {
            return _rpcConnector.MakeRequest<DecodeScriptResponse>(RpcMethods.decodescript, hexString);
        }

        public string DumpPrivKey(string bitcoinAddress)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.dumpprivkey, bitcoinAddress);
        }

        public void DumpWallet(string filename)
        {
            _rpcConnector.MakeRequest<string>(RpcMethods.dumpwallet, filename);
        }

        public string EncryptWallet(string passphrase)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.encryptwallet, passphrase);
        }

        public decimal EstimateFee(short nBlocks)
        {
            return _rpcConnector.MakeRequest<decimal>(RpcMethods.estimatefee, nBlocks);
        }

        public decimal EstimatePriority(short nBlocks)
        {
            return _rpcConnector.MakeRequest<decimal>(RpcMethods.estimatepriority, nBlocks);
        }

        public string GetAccount(string bitcoinAddress)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getaccount, bitcoinAddress);
        }

        public string GetAccountAddress(string account)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getaccountaddress, account);
        }

        public GetAddedNodeInfoResponse GetAddedNodeInfo(string dns, string node)
        {
            return string.IsNullOrWhiteSpace(node)
                       ? _rpcConnector.MakeRequest<GetAddedNodeInfoResponse>(RpcMethods.getaddednodeinfo, dns)
                       : _rpcConnector.MakeRequest<GetAddedNodeInfoResponse>(RpcMethods.getaddednodeinfo, dns, node);
        }

        public List<string> GetAddressesByAccount(string account)
        {
            return _rpcConnector.MakeRequest<List<string>>(RpcMethods.getaddressesbyaccount, account);
        }

        public decimal GetBalance(string account, int minConf, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<decimal>(RpcMethods.getbalance, (string.IsNullOrWhiteSpace(account) ? "*" : account), minConf)
                : _rpcConnector.MakeRequest<decimal>(RpcMethods.getbalance, (string.IsNullOrWhiteSpace(account) ? "*" : account), minConf, includeWatchonly);
        }

        public string GetBestBlockHash()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getbestblockhash);
        }

        public GetBlockResponse GetBlock(string hash, bool verbose)
        {
            return _rpcConnector.MakeRequest<GetBlockResponse>(RpcMethods.getblock, hash, verbose);
        }

        public GetBlockchainInfoResponse GetBlockchainInfo()
        {
            return _rpcConnector.MakeRequest<GetBlockchainInfoResponse>(RpcMethods.getblockchaininfo);
        }

        public uint GetBlockCount()
        {
            return _rpcConnector.MakeRequest<uint>(RpcMethods.getblockcount);
        }

        public string GetBlockHash(long index)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getblockhash, index);
        }

        public GetBlockTemplateResponse GetBlockTemplate(params object[] parameters)
        {
            return parameters == null
                       ? _rpcConnector.MakeRequest<GetBlockTemplateResponse>(RpcMethods.getblocktemplate)
                       : _rpcConnector.MakeRequest<GetBlockTemplateResponse>(RpcMethods.getblocktemplate, parameters);
        }

        public List<GetChainTipsResponse> GetChainTips()
        {
            return _rpcConnector.MakeRequest<List<GetChainTipsResponse>>(RpcMethods.getchaintips);
        }

        public int GetConnectionCount()
        {
            return _rpcConnector.MakeRequest<int>(RpcMethods.getconnectioncount);
        }

        public double GetDifficulty()
        {
            return _rpcConnector.MakeRequest<double>(RpcMethods.getdifficulty);
        }

        public string GetGenerate()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getgenerate);
        }

        public int GetHashesPerSec()
        {
            return _rpcConnector.MakeRequest<int>(RpcMethods.gethashespersec);
        }

        [Obsolete("Please use calls: GetWalletInfo(), GetBlockchainInfo() and GetNetworkInfo() instead")]
        public GetInfoResponse GetInfo()
        {
            return _rpcConnector.MakeRequest<GetInfoResponse>(RpcMethods.getinfo);
        }

        public GetMemPoolInfoResponse GetMemPoolInfo()
        {
            return _rpcConnector.MakeRequest<GetMemPoolInfoResponse>(RpcMethods.getmempoolinfo);
        }

        public GetMiningInfoResponse GetMiningInfo()
        {
            return _rpcConnector.MakeRequest<GetMiningInfoResponse>(RpcMethods.getmininginfo);
        }

        public GetNetTotalsResponse GetNetTotals()
        {
            return _rpcConnector.MakeRequest<GetNetTotalsResponse>(RpcMethods.getnettotals);
        }

        public ulong GetNetworkHashPs(uint blocks, long height)
        {
            return _rpcConnector.MakeRequest<ulong>(RpcMethods.getnetworkhashps);
        }

        public GetNetworkInfoResponse GetNetworkInfo()
        {
            return _rpcConnector.MakeRequest<GetNetworkInfoResponse>(RpcMethods.getnetworkinfo);
        }

        public string GetNewAddress(string account)
        {
            return string.IsNullOrWhiteSpace(account)
                       ? _rpcConnector.MakeRequest<string>(RpcMethods.getnewaddress)
                       : _rpcConnector.MakeRequest<string>(RpcMethods.getnewaddress, account);
        }

        public List<GetPeerInfoResponse> GetPeerInfo()
        {
            return _rpcConnector.MakeRequest<List<GetPeerInfoResponse>>(RpcMethods.getpeerinfo);
        }

        public GetRawMemPoolResponse GetRawMemPool(bool verbose)
        {
            GetRawMemPoolResponse getRawMemPoolResponse = new GetRawMemPoolResponse
            {
                IsVerbose = verbose
            };

            object rpcResponse = _rpcConnector.MakeRequest<object>(RpcMethods.getrawmempool, verbose);

            if (!verbose)
            {
                JArray rpcResponseAsArray = (JArray)rpcResponse;

                foreach (string txId in rpcResponseAsArray)
                {
                    getRawMemPoolResponse.TxIds.Add(txId);
                }

                return getRawMemPoolResponse;
            }

            IList<KeyValuePair<string, JToken>> rpcResponseAsKvp = (new EnumerableQuery<KeyValuePair<string, JToken>>(((JObject)(rpcResponse)))).ToList();
            IList<JToken> children = JObject.Parse(rpcResponse.ToString()).Children().ToList();

            for (int i = 0; i < children.Count(); i++)
            {
                GetRawMemPoolVerboseResponse getRawMemPoolVerboseResponse = new GetRawMemPoolVerboseResponse
                {
                    TxId = rpcResponseAsKvp[i].Key
                };

                getRawMemPoolResponse.TxIds.Add(getRawMemPoolVerboseResponse.TxId);

                foreach (JProperty property in children[i].SelectMany(grandChild => grandChild.OfType<JProperty>()))
                {
                    switch (property.Name)
                    {
                        case "currentpriority":

                            double currentPriority;

                            if (double.TryParse(property.Value.ToString(), out currentPriority))
                            {
                                getRawMemPoolVerboseResponse.CurrentPriority = currentPriority;
                            }

                            break;

                        case "depends":

                            foreach (JToken jToken in property.Value)
                            {
                                getRawMemPoolVerboseResponse.Depends.Add(jToken.Value<string>());
                            }

                            break;

                        case "fee":

                            decimal fee;

                            if (decimal.TryParse(property.Value.ToString(), out fee))
                            {
                                getRawMemPoolVerboseResponse.Fee = fee;
                            }

                            break;

                        case "height":

                            if (long.TryParse(property.Value.ToString(), out long height))
                            {
                                getRawMemPoolVerboseResponse.Height = height;
                            }

                            break;

                        case "size":

                            int size;

                            if (int.TryParse(property.Value.ToString(), out size))
                            {
                                getRawMemPoolVerboseResponse.Size = size;
                            }

                            break;

                        case "startingpriority":

                            double startingPriority;

                            if (double.TryParse(property.Value.ToString(), out startingPriority))
                            {
                                getRawMemPoolVerboseResponse.StartingPriority = startingPriority;
                            }

                            break;

                        case "time":

                            if (ulong.TryParse(property.Value.ToString(), out ulong time))
                            {
                                getRawMemPoolVerboseResponse.Time = time;
                            }

                            break;

                        default:

                            throw new Exception("Unkown property: " + property.Name + " in GetRawMemPool()");
                    }
                }
                getRawMemPoolResponse.VerboseResponses.Add(getRawMemPoolVerboseResponse);
            }
            return getRawMemPoolResponse;
        }

        public string GetRawChangeAddress()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getrawchangeaddress);
        }

        public GetRawTransactionResponse GetRawTransaction(string txId, int verbose)
        {
            if (verbose == 0)
            {
                return new GetRawTransactionResponse { Hex = _rpcConnector.MakeRequest<string>(RpcMethods.getrawtransaction, txId, verbose) };
            }

            if (verbose == 1)
            {
                return _rpcConnector.MakeRequest<GetRawTransactionResponse>(RpcMethods.getrawtransaction, txId, verbose);
            }

            throw new Exception("Invalid verbose value: " + verbose + " in GetRawTransaction()!");
        }

        public string GetReceivedByAccount(string account, int minConf)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getreceivedbyaccount, account, minConf);
        }

        public string GetReceivedByAddress(string bitcoinAddress, int minConf)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.getreceivedbyaddress, bitcoinAddress, minConf);
        }

        public GetTransactionResponse GetTransaction(string txId, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<GetTransactionResponse>(RpcMethods.gettransaction, txId)
                : _rpcConnector.MakeRequest<GetTransactionResponse>(RpcMethods.gettransaction, txId, includeWatchonly);
        }

        public GetTransactionResponse GetTxOut(string txId, int n, bool includeMemPool)
        {
            return _rpcConnector.MakeRequest<GetTransactionResponse>(RpcMethods.gettxout, txId, n, includeMemPool);
        }

        public GetTxOutSetInfoResponse GetTxOutSetInfo()
        {
            return _rpcConnector.MakeRequest<GetTxOutSetInfoResponse>(RpcMethods.gettxoutsetinfo);
        }

        public decimal GetUnconfirmedBalance()
        {
            return _rpcConnector.MakeRequest<decimal>(RpcMethods.getunconfirmedbalance);
        }

        public GetWalletInfoResponse GetWalletInfo()
        {
            return _rpcConnector.MakeRequest<GetWalletInfoResponse>(RpcMethods.getwalletinfo);
        }

        public string Help(string command)
        {
            return string.IsNullOrWhiteSpace(command)
                       ? _rpcConnector.MakeRequest<string>(RpcMethods.help)
                       : _rpcConnector.MakeRequest<string>(RpcMethods.help, command);
        }

        public void ImportAddress(string address, string label, bool rescan)
        {
            _rpcConnector.MakeRequest<string>(RpcMethods.importaddress, address, label, rescan);
        }

        public string ImportPrivKey(string privateKey, string label, bool rescan)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.importprivkey, privateKey, label, rescan);
        }

        public void ImportWallet(string filename)
        {
            _rpcConnector.MakeRequest<string>(RpcMethods.importwallet, filename);
        }

        public string KeyPoolRefill(uint newSize)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.keypoolrefill, newSize);
        }

        public Dictionary<string, decimal> ListAccounts(int minConf, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<Dictionary<string, decimal>>(RpcMethods.listaccounts, minConf)
                : _rpcConnector.MakeRequest<Dictionary<string, decimal>>(RpcMethods.listaccounts, minConf, includeWatchonly);
        }

        public List<List<ListAddressGroupingsResponse>> ListAddressGroupings()
        {
            List<List<List<object>>> unstructuredResponse = _rpcConnector.MakeRequest<List<List<List<object>>>>(RpcMethods.listaddressgroupings);
            List<List<ListAddressGroupingsResponse>> structuredResponse = new List<List<ListAddressGroupingsResponse>>(unstructuredResponse.Count);

            for (int i = 0; i < unstructuredResponse.Count; i++)
            {
                for (int j = 0; j < unstructuredResponse[i].Count; j++)
                {
                    if (unstructuredResponse[i][j].Count > 1)
                    {
                        ListAddressGroupingsResponse response = new ListAddressGroupingsResponse
                        {
                            Address = unstructuredResponse[i][j][0].ToString()
                        };

                        bool conv_flag = false;
                        decimal balance = decimal.Zero;
                        string tmp_balance = unstructuredResponse[i][j][1].ToString().Trim();
                        if (tmp_balance.Contains("E"))
                            conv_flag = decimal.TryParse(unstructuredResponse[i][j][1].ToString(), System.Globalization.NumberStyles.Float, null, out balance);
                        else
                            conv_flag = decimal.TryParse(unstructuredResponse[i][j][1].ToString(), out balance);

                        if (conv_flag)
                            response.Balance = balance;

                        if (unstructuredResponse[i][j].Count > 2)
                        {
                            response.Account = unstructuredResponse[i][j][2].ToString();
                        }

                        if (structuredResponse.Count < i + 1)
                        {
                            structuredResponse.Add(new List<ListAddressGroupingsResponse>());
                        }

                        structuredResponse[i].Add(response);
                    }
                }
            }
            return structuredResponse;
        }

        public string ListLockUnspent()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.listlockunspent);
        }

        public List<ListReceivedByAccountResponse> ListReceivedByAccount(int minConf, bool includeEmpty, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<List<ListReceivedByAccountResponse>>(RpcMethods.listreceivedbyaccount, minConf, includeEmpty)
                : _rpcConnector.MakeRequest<List<ListReceivedByAccountResponse>>(RpcMethods.listreceivedbyaccount, minConf, includeEmpty, includeWatchonly);
        }

        public List<ListReceivedByAddressResponse> ListReceivedByAddress(int minConf, bool includeEmpty, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<List<ListReceivedByAddressResponse>>(RpcMethods.listreceivedbyaddress, minConf, includeEmpty)
                : _rpcConnector.MakeRequest<List<ListReceivedByAddressResponse>>(RpcMethods.listreceivedbyaddress, minConf, includeEmpty, includeWatchonly);
        }

        public ListSinceBlockResponse ListSinceBlock(string blockHash, int targetConfirmations, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<ListSinceBlockResponse>(RpcMethods.listsinceblock, (string.IsNullOrWhiteSpace(blockHash) ? "*" : blockHash), targetConfirmations)
                : _rpcConnector.MakeRequest<ListSinceBlockResponse>(RpcMethods.listsinceblock, (string.IsNullOrWhiteSpace(blockHash) ? "*" : blockHash), targetConfirmations, includeWatchonly);
        }

        public List<ListTransactionsResponse> ListTransactions(string account, int count, int from, bool? includeWatchonly)
        {
            return includeWatchonly == null
                ? _rpcConnector.MakeRequest<List<ListTransactionsResponse>>(RpcMethods.listtransactions, (string.IsNullOrWhiteSpace(account) ? "*" : account), count, from)
                : _rpcConnector.MakeRequest<List<ListTransactionsResponse>>(RpcMethods.listtransactions, (string.IsNullOrWhiteSpace(account) ? "*" : account), count, from, includeWatchonly);
        }

        public List<ListUnspentResponse> ListUnspent(int minConf, int maxConf, List<string> addresses)
        {
            return _rpcConnector.MakeRequest<List<ListUnspentResponse>>(RpcMethods.listunspent, minConf, maxConf, (addresses ?? new List<string>()));
        }

        public bool LockUnspent(bool unlock, IList<ListUnspentResponse> listUnspentResponses)
        {
            IList<Object> transactions = new List<object>();

            foreach (ListUnspentResponse listUnspentResponse in listUnspentResponses)
            {
                transactions.Add(new { txid = listUnspentResponse.TxId, Vout = listUnspentResponse.Vout });
            }

            return _rpcConnector.MakeRequest<bool>(RpcMethods.lockunspent, unlock, transactions.ToArray());
        }

        public bool Move(string fromAccount, string toAccount, decimal amount, int minConf, string comment)
        {
            return _rpcConnector.MakeRequest<bool>(RpcMethods.move, fromAccount, toAccount, amount, minConf, comment);
        }

        public void Ping()
        {
            _rpcConnector.MakeRequest<string>(RpcMethods.ping);
        }

        public bool PrioritiseTransaction(string txId, decimal priorityDelta, decimal feeDelta)
        {
            return _rpcConnector.MakeRequest<bool>(RpcMethods.prioritisetransaction, txId, priorityDelta, feeDelta);
        }

        public string SendFrom(string fromAccount, string toBitcoinAddress, decimal amount, int minConf, string comment, string commentTo)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.sendfrom, fromAccount, toBitcoinAddress, amount, minConf, comment, commentTo);
        }

        public string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf, string comment)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.sendmany, fromAccount, toBitcoinAddress, minConf, comment);
        }

        public string SendRawTransaction(string rawTransactionHexString, bool? allowHighFees)
        {
            return allowHighFees == null
                       ? _rpcConnector.MakeRequest<string>(RpcMethods.sendrawtransaction, rawTransactionHexString)
                       : _rpcConnector.MakeRequest<string>(RpcMethods.sendrawtransaction, rawTransactionHexString, allowHighFees);
        }

        public string SendToAddress(string bitcoinAddress, decimal amount, string comment, string commentTo)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.sendtoaddress, bitcoinAddress, amount, comment, commentTo);
        }

        public string SetAccount(string bitcoinAddress, string account)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.setaccount, bitcoinAddress, account);
        }

        public string SetGenerate(bool generate, short generatingProcessorsLimit)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.setgenerate, generate, generatingProcessorsLimit);
        }

        public string SetTxFee(decimal amount)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.settxfee, amount);
        }

        public string SignMessage(string bitcoinAddress, string message)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.signmessage, bitcoinAddress, message);
        }

        public SignRawTransactionResponse SignRawTransaction(SignRawTransactionRequest request)
        {
            #region default values

            if (request.Inputs.Count == 0)
            {
                request.Inputs = null;
            }

            if (string.IsNullOrWhiteSpace(request.SigHashType))
            {
                request.SigHashType = SigHashType.All;
            }

            if (request.PrivateKeys.Count == 0)
            {
                request.PrivateKeys = null;
            }

            #endregion

            return _rpcConnector.MakeRequest<SignRawTransactionResponse>(RpcMethods.signrawtransaction, request.RawTransactionHex, request.Inputs, request.PrivateKeys, request.SigHashType);
        }

        public string Stop()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.stop);
        }

        public string SubmitBlock(string hexData, params object[] parameters)
        {
            return parameters == null
                       ? _rpcConnector.MakeRequest<string>(RpcMethods.submitblock, hexData)
                       : _rpcConnector.MakeRequest<string>(RpcMethods.submitblock, hexData, parameters);
        }

        public ValidateAddressResponse ValidateAddress(string bitcoinAddress)
        {
            return _rpcConnector.MakeRequest<ValidateAddressResponse>(RpcMethods.validateaddress, bitcoinAddress);
        }

        public bool VerifyChain(short checkLevel, uint numBlocks)
        {
            return _rpcConnector.MakeRequest<bool>(RpcMethods.verifychain, checkLevel, numBlocks);
        }

        public string VerifyMessage(string bitcoinAddress, string signature, string message)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.verifymessage, bitcoinAddress, signature, message);
        }

        public string WalletLock()
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.walletlock);
        }

        public string WalletPassphrase(string passphrase, int timeoutInSeconds)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.walletpassphrase, passphrase, timeoutInSeconds);
        }

        public string WalletPassphraseChange(string oldPassphrase, string newPassphrase)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.walletpassphrasechange, oldPassphrase, newPassphrase);
        }

        #endregion

        #region RpcExtender Service

        //  Note: This will return funky results if the address in question along with its private key have been used to create a multisig address with unspent funds
        public decimal GetAddressBalance(string inWalletAddress, int minConf, bool validateAddressBeforeProcessing)
        {
            if (validateAddressBeforeProcessing)
            {
                ValidateAddressResponse validateAddressResponse = ValidateAddress(inWalletAddress);

                if (!validateAddressResponse.IsValid)
                {
                    throw new GetAddressBalanceException(string.Format("Address {0} is invalid!", inWalletAddress));
                }

                if (!validateAddressResponse.IsMine)
                {
                    throw new GetAddressBalanceException(string.Format("Address {0} is not an in-wallet address!", inWalletAddress));
                }
            }

            List<ListUnspentResponse> listUnspentResponses = ListUnspent(minConf, 9999999, new List<string>
                {
                    inWalletAddress
                });

            return listUnspentResponses.Any() ? listUnspentResponses.Sum(x => x.Amount) : 0;
        }

        public string GetImmutableTxId(string txId, bool getSha256Hash)
        {
            GetRawTransactionResponse response = GetRawTransaction(txId, 1);
            string text = response.Vin.First().TxId + "|" + response.Vin.First().Vout + "|" + response.Vout.First().Value;
            return getSha256Hash ? Hashing.GetSha256(text) : text;
        }

        //  Get a rough estimate on fees for non-free txs, depending on the total number of tx inputs and outputs
        [Obsolete("Please don't use this method to calculate tx fees, its purpose is to provide a rough estimate only")]
        public decimal GetMinimumNonZeroTransactionFeeEstimate(short numberOfInputs = 1, short numberOfOutputs = 1)
        {
            CreateRawTransactionRequest rawTransactionRequest = new CreateRawTransactionRequest(new List<CreateRawTransactionInput>(numberOfInputs), new Dictionary<string, decimal>(numberOfOutputs));

            for (short i = 0; i < numberOfInputs; i++)
            {
                rawTransactionRequest.AddInput(new CreateRawTransactionInput { TxId = "dummyTxId" + i.ToString(CultureInfo.InvariantCulture), Vout = i });
            }

            for (short i = 0; i < numberOfOutputs; i++)
            {
                rawTransactionRequest.AddOutput(new CreateRawTransactionOutput { Address = "dummyAddress" + i.ToString(CultureInfo.InvariantCulture), Amount = i + 1 });
            }

            return GetTransactionFee(rawTransactionRequest, false, true);
        }

        public Dictionary<string, string> GetMyPublicAndPrivateKeyPairs()
        {
            const short secondsToUnlockTheWallet = 30;
            Dictionary<string, string> keyPairs = new Dictionary<string, string>();
            WalletPassphrase(Parameters.WalletPassword, secondsToUnlockTheWallet);
            List<ListReceivedByAddressResponse> myAddresses = (this as ICoinService).ListReceivedByAddress(0, true);

            foreach (ListReceivedByAddressResponse listReceivedByAddressResponse in myAddresses)
            {
                ValidateAddressResponse validateAddressResponse = ValidateAddress(listReceivedByAddressResponse.Address);

                if (validateAddressResponse.IsMine && validateAddressResponse.IsValid && !validateAddressResponse.IsScript)
                {
                    string privateKey = DumpPrivKey(listReceivedByAddressResponse.Address);
                    keyPairs.Add(validateAddressResponse.PubKey, privateKey);
                }
            }

            WalletLock();
            return keyPairs;
        }

        //  Note: As RPC's gettransaction works only for in-wallet transactions this had to be extended so it will work for every single transaction.
        public DecodeRawTransactionResponse GetPublicTransaction(string txId)
        {
            string rawTransaction = GetRawTransaction(txId, 0).Hex;
            return DecodeRawTransaction(rawTransaction);
        }

        [Obsolete("Please use EstimateFee() instead. You can however keep on using this method until the network fully adjusts to the new rules on fee calculation")]
        public decimal GetTransactionFee(CreateRawTransactionRequest transaction, bool checkIfTransactionQualifiesForFreeRelay, bool enforceMinimumTransactionFeePolicy)
        {
            if (checkIfTransactionQualifiesForFreeRelay && IsTransactionFree(transaction))
            {
                return 0;
            }

            decimal transactionSizeInBytes = GetTransactionSizeInBytes(transaction);
            decimal transactionFee = ((transactionSizeInBytes / Parameters.FreeTransactionMaximumSizeInBytes) + (transactionSizeInBytes % Parameters.FreeTransactionMaximumSizeInBytes == 0 ? 0 : 1)) * Parameters.FeePerThousandBytesInCoins;

            if (transactionFee.GetNumberOfDecimalPlaces() > Parameters.CoinsPerBaseUnit.GetNumberOfDecimalPlaces())
            {
                transactionFee = decimal.Round(transactionFee, Parameters.CoinsPerBaseUnit.GetNumberOfDecimalPlaces(), MidpointRounding.AwayFromZero);
            }

            if (enforceMinimumTransactionFeePolicy && Parameters.MinimumTransactionFeeInCoins != 0 && transactionFee < Parameters.MinimumTransactionFeeInCoins)
            {
                transactionFee = Parameters.MinimumTransactionFeeInCoins;
            }

            return transactionFee;
        }

        public GetRawTransactionResponse GetRawTxFromImmutableTxId(string rigidTxId, int listTransactionsCount, int listTransactionsFrom, bool getRawTransactionVersbose, bool rigidTxIdIsSha256)
        {
            List<ListTransactionsResponse> allTransactions = (this as ICoinService).ListTransactions("*", listTransactionsCount, listTransactionsFrom);

            return (from listTransactionsResponse in allTransactions
                    where rigidTxId == GetImmutableTxId(listTransactionsResponse.TxId, rigidTxIdIsSha256)
                    select GetRawTransaction(listTransactionsResponse.TxId, getRawTransactionVersbose ? 1 : 0)).FirstOrDefault();
        }

        public decimal GetTransactionPriority(CreateRawTransactionRequest transaction)
        {
            if (transaction.Inputs.Count == 0)
            {
                return 0;
            }

            List<ListUnspentResponse> unspentInputs = (this as ICoinService).ListUnspent(0).ToList();
            decimal sumOfInputsValueInBaseUnitsMultipliedByTheirAge = transaction.Inputs.Select(input => unspentInputs.First(x => x.TxId == input.TxId)).Select(unspentResponse => (unspentResponse.Amount * Parameters.OneCoinInBaseUnits) * unspentResponse.Confirmations).Sum();
            return sumOfInputsValueInBaseUnitsMultipliedByTheirAge / GetTransactionSizeInBytes(transaction);
        }

        public decimal GetTransactionPriority(IList<ListUnspentResponse> transactionInputs, int numberOfOutputs)
        {
            if (transactionInputs.Count == 0)
            {
                return 0;
            }

            return transactionInputs.Sum(input => input.Amount * Parameters.OneCoinInBaseUnits * input.Confirmations) / GetTransactionSizeInBytes(transactionInputs.Count, numberOfOutputs);
        }

        //  Note: Be careful when using GetTransactionSenderAddress() as it just gives you an address owned by someone who previously controlled the transaction's outputs
        //  which might not actually be the sender (e.g. for e-wallets) and who may not intend to receive anything there in the first place. 
        [Obsolete("Please don't use this method in production enviroment, it's for testing purposes only")]
        public string GetTransactionSenderAddress(string txId)
        {
            string rawTransaction = GetRawTransaction(txId, 0).Hex;
            DecodeRawTransactionResponse decodedRawTransaction = DecodeRawTransaction(rawTransaction);
            List<Vin> transactionInputs = decodedRawTransaction.Vin;
            string rawTransactionHex = GetRawTransaction(transactionInputs[0].TxId, 0).Hex;
            DecodeRawTransactionResponse inputDecodedRawTransaction = DecodeRawTransaction(rawTransactionHex);
            List<Vout> vouts = inputDecodedRawTransaction.Vout;
            return vouts[0].ScriptPubKey.Addresses[0];
        }

        public int GetTransactionSizeInBytes(CreateRawTransactionRequest transaction)
        {
            return GetTransactionSizeInBytes(transaction.Inputs.Count, transaction.Outputs.Count);
        }

        public int GetTransactionSizeInBytes(int numberOfInputs, int numberOfOutputs)
        {
            return numberOfInputs * Parameters.TransactionSizeBytesContributedByEachInput
                   + numberOfOutputs * Parameters.TransactionSizeBytesContributedByEachOutput
                   + Parameters.TransactionSizeFixedExtraSizeInBytes
                   + numberOfInputs;
        }

        public bool IsInWalletTransaction(string txId)
        {
            //  Note: This might not be efficient if iterated, consider caching ListTransactions' results.
            return (this as ICoinService).ListTransactions(null, int.MaxValue).Any(listTransactionsResponse => listTransactionsResponse.TxId == txId);
        }

        public bool IsTransactionFree(CreateRawTransactionRequest transaction)
        {
            return transaction.Outputs.Any(x => x.Value < Parameters.FreeTransactionMinimumOutputAmountInCoins)
                   && GetTransactionSizeInBytes(transaction) < Parameters.FreeTransactionMaximumSizeInBytes
                   && GetTransactionPriority(transaction) > Parameters.FreeTransactionMinimumPriority;
        }

        public bool IsTransactionFree(IList<ListUnspentResponse> transactionInputs, int numberOfOutputs, decimal minimumAmountAmongOutputs)
        {
            return minimumAmountAmongOutputs < Parameters.FreeTransactionMinimumOutputAmountInCoins
                   && GetTransactionSizeInBytes(transactionInputs.Count, numberOfOutputs) < Parameters.FreeTransactionMaximumSizeInBytes
                   && GetTransactionPriority(transactionInputs, numberOfOutputs) > Parameters.FreeTransactionMinimumPriority;
        }

        public bool IsWalletEncrypted()
        {
            return !Help(RpcMethods.walletlock.ToString()).Contains("unknown command");
        }

        #endregion
    }
}