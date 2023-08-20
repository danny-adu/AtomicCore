using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 获取当前钱包内的USDT余额
    /// </summary>
    public class OMNI_WalletAddressBalanceResponse
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public List<OMNI_TokenAddressBalance> balances { get; set; }
    }

    /// <summary>
    /// 代币余额度模型
    /// </summary>
    public class OMNI_TokenAddressBalance
    {
        /// <summary>
        /// 代币令牌
        /// </summary>
        public ulong propertyid { get; set; }

        /// <summary>
        /// 代币名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }

        /// <summary>
        /// 保留
        /// </summary>
        public decimal reserved { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public decimal frozen { get; set; }
    }
}
