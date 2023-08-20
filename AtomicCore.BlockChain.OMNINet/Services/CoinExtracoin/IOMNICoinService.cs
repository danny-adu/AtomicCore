using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// OMIN-USDT
    /// </summary>
    public interface IOMNICoinService : ICoinService, IExtracoinConstants
    {
        /// <summary>
        /// 获取OMNI信息
        /// </summary>
        /// <returns></returns>
        OMNI_GetInfoResponse GetOMNIInfo();

        /// <summary>
        /// 获取USDT的额度
        /// </summary>
        /// <param name="address"></param>
        /// <param name="propertyid"></param>
        /// <returns></returns>
        OMNI_AddressBalanceResponse GetOMINBalance(string address, int propertyid = 31);

        /// <summary>
        /// 获取钱包内的USDT地址余额列表
        /// </summary>
        /// <returns></returns>
        List<OMNI_WalletAddressBalanceResponse> GetOMNIWalletAddressBalances();

        /// <summary>
        /// 获取指定某一个区块下的OMNI交易TXID
        /// </summary>
        /// <param name="index">区块高度</param>
        /// <returns></returns>
        string[] GetOMNIListBlockTransactions(ulong index);

        /// <summary>
        /// 根据TXID获取交易信息
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        OMNI_GetTransactionResponse GetOMNITransaction(string txid);

        /// <summary>
        /// 资产归集指定手续费地址支付手续费
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="feeAddress"></param>
        /// <param name="amount"></param>
        /// <param name="propertyid"></param>
        /// <returns></returns>
        string OMNIFundedSend(string fromAddress, string toAddress, string feeAddress, decimal amount, int propertyid = 31);

        /// <summary>
        /// 资产归集指定手续费支付手续费（发送地址的所有代币全部归集）
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="feeAddress"></param>
        /// <param name="ecosystem"></param>
        /// <returns></returns>
        string OMNIFundedSendALL(string fromAddress, string toAddress, string feeAddress, int ecosystem = 1);

        /// <summary>
        /// 发送OMNI RawTransaction
        /// </summary>
        /// <param name="rawTransactionHexString"></param>
        /// <param name="maxfeerate"></param>
        /// <returns></returns>
        string OMNISendRawTransaction(string rawTransactionHexString, decimal maxfeerate = decimal.Zero);
    }
}
