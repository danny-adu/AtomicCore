using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// OMNI-USDT
    /// </summary>
    public sealed class OMNICoinService : CoinService, IOMNICoinService
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="useTestnet">是否使用测试线路</param>
        public OMNICoinService(bool useTestnet = false)
            : base(useTestnet)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="daemonUrl">服务节点地址</param>
        /// <param name="rpcUsername">RPC接口用户名</param>
        /// <param name="rpcPassword">RPC接口密码</param>
        /// <param name="walletPassword">钱包密码</param>
        public OMNICoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword)
            : base(daemonUrl, rpcUsername, rpcPassword, walletPassword)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="daemonUrl">服务节点地址</param>
        /// <param name="rpcUsername">RPC接口用户名</param>
        /// <param name="rpcPassword">RPC接口密码</param>
        /// <param name="walletPassword">钱包密码</param>
        /// <param name="rpcRequestTimeoutInSeconds">RPC请求超时时间</param>
        public OMNICoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword, short rpcRequestTimeoutInSeconds)
            : base(daemonUrl, rpcUsername, rpcPassword, walletPassword, rpcRequestTimeoutInSeconds)
        {
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 常量定义（继续点，有很多）
        /// </summary>
        public ExtracoinConstants.Constants Constants
        {
            get { return ExtracoinConstants.Constants.Instance; }
        }

        #endregion

        #region IUSDTCoinService Methods

        public OMNI_GetInfoResponse GetOMNIInfo()
        {
            return _rpcConnector.MakeRequest<OMNI_GetInfoResponse>(RpcMethods.omni_getinfo);
        }

        /// <summary>
        /// 获取USDT的额度
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OMNI_AddressBalanceResponse GetOMINBalance(string address, int propertyid = 31)
        {
            return _rpcConnector.MakeRequest<OMNI_AddressBalanceResponse>(RpcMethods.omni_getbalance, address, propertyid);
        }

        /// <summary>
        /// 获取钱包内的USDT地址余额列表
        /// </summary>
        /// <returns></returns>
        public List<OMNI_WalletAddressBalanceResponse> GetOMNIWalletAddressBalances()
        {
            return _rpcConnector.MakeRequest<List<OMNI_WalletAddressBalanceResponse>>(RpcMethods.omni_getwalletaddressbalances);
        }

        /// <summary>
        /// 获取区块中的交易TXID数组
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string[] GetOMNIListBlockTransactions(ulong index)
        {
            return _rpcConnector.MakeRequest<string[]>(RpcMethods.omni_listblocktransactions, index);
        }

        /// <summary>
        /// 获取某一个交易信息
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        public OMNI_GetTransactionResponse GetOMNITransaction(string txid)
        {
            return _rpcConnector.MakeRequest<OMNI_GetTransactionResponse>(RpcMethods.omni_gettransaction, txid);
        }

        /// <summary>
        /// 资产归集指定手续费地址支付手续费
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="feeAddress"></param>
        /// <param name="amount"></param>
        /// <param name="propertyid"></param>
        /// <returns></returns>
        public string OMNIFundedSend(string fromAddress, string toAddress, string feeAddress, decimal amount, int propertyid = 31)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.omni_funded_send, fromAddress, toAddress, propertyid, amount.ToString(), feeAddress);
        }

        /// <summary>
        /// 资产归集指定手续费支付手续费（发送地址的所有代币全部归集）
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="feeAddress"></param>
        /// <param name="ecosystem"></param>
        /// <returns></returns>
        public string OMNIFundedSendALL(string fromAddress, string toAddress, string feeAddress, int ecosystem = 1)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.omni_funded_sendall, fromAddress, toAddress, ecosystem, feeAddress);
        }

        OMNI_GetInfoResponse IOMNICoinService.GetOMNIInfo()
        {
            throw new System.NotImplementedException();
        }

        OMNI_AddressBalanceResponse IOMNICoinService.GetOMINBalance(string address, int propertyid)
        {
            throw new System.NotImplementedException();
        }

        List<OMNI_WalletAddressBalanceResponse> IOMNICoinService.GetOMNIWalletAddressBalances()
        {
            throw new System.NotImplementedException();
        }

        string[] IOMNICoinService.GetOMNIListBlockTransactions(ulong index)
        {
            throw new System.NotImplementedException();
        }

        OMNI_GetTransactionResponse IOMNICoinService.GetOMNITransaction(string txid)
        {
            throw new System.NotImplementedException();
        }

        string IOMNICoinService.OMNIFundedSend(string fromAddress, string toAddress, string feeAddress, decimal amount, int propertyid)
        {
            throw new System.NotImplementedException();
        }

        string IOMNICoinService.OMNIFundedSendALL(string fromAddress, string toAddress, string feeAddress, int ecosystem)
        {
            throw new System.NotImplementedException();
        }

        public string OMNISendRawTransaction(string rawTransactionHexString, decimal maxfeerate = 0)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.sendrawtransaction, rawTransactionHexString, maxfeerate);
        }

        #endregion
    }
}
