// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// IRpcService
    /// </summary>
    public interface IRpcService
    {
        string AddMultiSigAddress(int nRquired, List<string> publicKeys, string account = null);
        string AddNode(string node, NodeAction action);
        string BackupWallet(string destination);
        CreateMultiSigResponse CreateMultiSig(int nRquired, List<string> publicKeys);
        string CreateRawTransaction(CreateRawTransactionRequest rawTransaction);
        DecodeRawTransactionResponse DecodeRawTransaction(string rawTransactionHexString);
        DecodeScriptResponse DecodeScript(string hexString);
        string DumpPrivKey(string bitcoinAddress);
        void DumpWallet(string filename);
        string EncryptWallet(string passphrame);
        decimal EstimateFee(short nBlocks);
        decimal EstimatePriority(short nBlocks);
        /// <summary>
        /// 根据账户地址获取本地地址簿的地址对应的标签（昵称）
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <returns></returns>
        string GetAccount(string bitcoinAddress);
        /// <summary>
        /// 根据账户别名获取本地地址簿的地址信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        string GetAccountAddress(string account);
        GetAddedNodeInfoResponse GetAddedNodeInfo(string dns, string node = null);
        List<string> GetAddressesByAccount(string account);
        decimal GetBalance(string account = null, int minConf = 1, bool? includeWatchonly = null);
        string GetBestBlockHash();
        GetBlockResponse GetBlock(string hash, bool verbose = true);
        GetBlockchainInfoResponse GetBlockchainInfo();
        /// <summary>
        /// 获取累计已挖出的块数量总和
        /// </summary>
        /// <returns></returns>
        uint GetBlockCount();
        string GetBlockHash(long index);
        GetBlockTemplateResponse GetBlockTemplate(params object[] parameters);
        List<GetChainTipsResponse> GetChainTips();
        /// <summary>
        /// 获取连接的客户端数量总和
        /// </summary>
        /// <returns></returns>
        int GetConnectionCount();
        double GetDifficulty();
        string GetGenerate();
        int GetHashesPerSec();

        /// <summary>
        /// 获取当前节点的用户的信息
        /// </summary>
        /// <returns></returns>
        GetInfoResponse GetInfo();
        GetMemPoolInfoResponse GetMemPoolInfo();
        GetMiningInfoResponse GetMiningInfo();
        GetNetTotalsResponse GetNetTotals();
        ulong GetNetworkHashPs(uint blocks = 120, long height = -1);
        GetNetworkInfoResponse GetNetworkInfo();
        string GetNewAddress(string account = "");
        List<GetPeerInfoResponse> GetPeerInfo();
        string GetRawChangeAddress();
        GetRawMemPoolResponse GetRawMemPool(bool verbose = false);
        GetRawTransactionResponse GetRawTransaction(string txId, int verbose = 0);
        string GetReceivedByAccount(string account, int minConf = 1);
        string GetReceivedByAddress(string bitcoinAddress, int minConf = 1);
        /// <summary>
        /// 获取指定交易的详情信息
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="includeWatchonly"></param>
        /// <returns></returns>
        GetTransactionResponse GetTransaction(string txId, bool? includeWatchonly = null);
        GetTransactionResponse GetTxOut(string txId, int n, bool includeMemPool = true);
        GetTxOutSetInfoResponse GetTxOutSetInfo();
        /// <summary>
        /// 获取未确认余额
        /// </summary>
        /// <returns></returns>
        decimal GetUnconfirmedBalance();
        GetWalletInfoResponse GetWalletInfo();
        /// <summary>
        /// 获取帮助信息
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string Help(string command = null);

        /// <summary>
        /// 导出地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="label"></param>
        /// <param name="rescan"></param>
        void ImportAddress(string address, string label = null, bool rescan = true);
        /// <summary>
        /// 导出私匙
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="label"></param>
        /// <param name="rescan"></param>
        /// <returns></returns>
        string ImportPrivKey(string privateKey, string label = null, bool rescan = true);
        /// <summary>
        /// 导出钱包
        /// </summary>
        /// <param name="filename"></param>
        void ImportWallet(string filename);
        string KeyPoolRefill(uint newSize = 100);
        Dictionary<string, decimal> ListAccounts(int minConf = 1, bool? includeWatchonly = null);
        List<List<ListAddressGroupingsResponse>> ListAddressGroupings();
        string ListLockUnspent();
        List<ListReceivedByAccountResponse> ListReceivedByAccount(int minConf = 1, bool includeEmpty = false, bool? includeWatchonly = null);
        List<ListReceivedByAddressResponse> ListReceivedByAddress(int minConf = 1, bool includeEmpty = false, bool? includeWatchonly = null);
        ListSinceBlockResponse ListSinceBlock(string blockHash = null, int targetConfirmations = 1, bool? includeWatchonly = null);

        /// <summary>
        /// 获取我的交易数据
        /// </summary>
        /// <param name="account">指定与其帐号的,null标识通配符*所有</param>
        /// <param name="count">需要获取的交易单数量</param>
        /// <param name="from">分页页码</param>
        /// <param name="includeWatchonly"></param>
        /// <returns></returns>
        List<ListTransactionsResponse> ListTransactions(string account = null, int count = 10, int from = 0, bool? includeWatchonly = null);
        List<ListUnspentResponse> ListUnspent(int minConf = 1, int maxConf = 9999999, List<string> addresses = null);
        bool LockUnspent(bool unlock, IList<ListUnspentResponse> listUnspentResponses);
        bool Move(string fromAccount, string toAccount, decimal amount, int minConf = 1, string comment = "");
        /// <summary>
        /// 钱包节点的ping服务,用于测试网络是否通
        /// </summary>
        void Ping();
        /// <summary>
        /// 优先交易
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="priorityDelta"></param>
        /// <param name="feeDelta"></param>
        /// <returns></returns>
        bool PrioritiseTransaction(string txId, decimal priorityDelta, decimal feeDelta);
        string SendFrom(string fromAccount, string toBitcoinAddress, decimal amount, int minConf = 1, string comment = null, string commentTo = null);
        string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf = 1, string comment = null);
        string SendRawTransaction(string rawTransactionHexString, bool? allowHighFees = false);
        /// <summary>
        /// 给指定的账户上进行转账
        /// </summary>
        /// <param name="bitcoinAddress">指定转入的账户地址</param>
        /// <param name="amount">转账金额</param>
        /// <param name="comment">设置交易评价</param>
        /// <param name="commentTo"></param>
        /// <returns></returns>
        string SendToAddress(string bitcoinAddress, decimal amount, string comment = null, string commentTo = null);
        /// <summary>
        /// 设置账户的地址对应的别名,例如:设置地址JZ4up1WhXVxMwWZBKKXfFdKs6pWb6ZM7h6标记为桢姐
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        string SetAccount(string bitcoinAddress, string account);
        string SetGenerate(bool generate, short generatingProcessorsLimit);
        string SetTxFee(decimal amount);
        /// <summary>
        /// 设置地址签名
        /// </summary>
        /// <param name="bitcoinAddress">地址信息</param>
        /// <param name="message">签名信息</param>
        /// <returns></returns>
        string SignMessage(string bitcoinAddress, string message);
        SignRawTransactionResponse SignRawTransaction(SignRawTransactionRequest signRawTransactionRequest);
        string Stop();
        string SubmitBlock(string hexData, params Object[] parameters);

        /// <summary>
        /// 验证地址
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <returns></returns>
        ValidateAddressResponse ValidateAddress(string bitcoinAddress);

        /// <summary>
        /// 验证链
        /// </summary>
        /// <param name="checkLevel"></param>
        /// <param name="numBlocks"></param>
        /// <returns></returns>
        bool VerifyChain(short checkLevel = 3, uint numBlocks = 288); //  Note: numBlocks: 0 => ALL
        /// <summary>
        /// 验证消息
        /// </summary>
        /// <param name="bitcoinAddress"></param>
        /// <param name="signature"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        string VerifyMessage(string bitcoinAddress, string signature, string message);

        /// <summary>
        /// 钱包锁定
        /// </summary>
        /// <returns></returns>
        string WalletLock();
        /// <summary>
        /// 钱包密码设置
        /// </summary>
        /// <param name="passphrase"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        string WalletPassphrase(string passphrase, int timeoutInSeconds);
        /// <summary>
        /// 钱包密码变更
        /// </summary>
        /// <param name="oldPassphrase"></param>
        /// <param name="newPassphrase"></param>
        /// <returns></returns>
        string WalletPassphraseChange(string oldPassphrase, string newPassphrase);



        /* 
         * IRpcExtenderService 
         */
        decimal GetAddressBalance(string inWalletAddress, int minConf = 0, bool validateAddressBeforeProcessing = true);
        decimal GetMinimumNonZeroTransactionFeeEstimate(short numberOfInputs = 1, short numberOfOutputs = 1);
        Dictionary<string, string> GetMyPublicAndPrivateKeyPairs();
        DecodeRawTransactionResponse GetPublicTransaction(string txId);
        decimal GetTransactionFee(CreateRawTransactionRequest createRawTransactionRequest, bool checkIfTransactionQualifiesForFreeRelay = true, bool enforceMinimumTransactionFeePolicy = true);
        decimal GetTransactionPriority(CreateRawTransactionRequest createRawTransactionRequest);
        decimal GetTransactionPriority(IList<ListUnspentResponse> transactionInputs, int numberOfOutputs);
        string GetTransactionSenderAddress(string txId);
        int GetTransactionSizeInBytes(CreateRawTransactionRequest createRawTransactionRequest);
        int GetTransactionSizeInBytes(int numberOfInputs, int numberOfOutputs);
        GetRawTransactionResponse GetRawTxFromImmutableTxId(string rigidTxId, int listTransactionsCount = int.MaxValue, int listTransactionsFrom = 0, bool getRawTransactionVersbose = true, bool rigidTxIdIsSha256 = false);
        string GetImmutableTxId(string txId, bool getSha256Hash = false);
        bool IsInWalletTransaction(string txId);
        bool IsTransactionFree(CreateRawTransactionRequest createRawTransactionRequest);
        bool IsTransactionFree(IList<ListUnspentResponse> transactionInputs, int numberOfOutputs, decimal minimumAmountAmongOutputs);
        bool IsWalletEncrypted();
    }
}