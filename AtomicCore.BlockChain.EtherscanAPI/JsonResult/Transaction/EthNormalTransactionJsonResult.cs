using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// 交易信息JSON结果集
    /// </summary>
    public class EthNormalTransactionJsonResult : EthTransactionJsonResult
    {
        /// <summary>
        /// 地址Nonce
        /// </summary>
        [JsonProperty("nonce")]
        public int Nonce { get; set; }

        /// <summary>
        ///  交易所在区块的索引位
        /// </summary>
        [JsonProperty("transactionIndex")]
        public int TransactionIndex { get; set; }

        /// <summary>
        /// 区块哈希
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
    }
}
