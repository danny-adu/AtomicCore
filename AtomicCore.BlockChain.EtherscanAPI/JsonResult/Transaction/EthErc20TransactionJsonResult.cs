using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// ERC20交易记录
    /// </summary>
    public class EthErc20TransactionJsonResult : EthNormalTransactionJsonResult
    {
        /// <summary>
        /// 合约代币名称
        /// </summary>
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }

        /// <summary>
        /// 合约代币符号
        /// </summary>
        [JsonProperty("tokenSymbol")]
        public string TokenSymbol { get; set; }

        /// <summary>
        /// 合约代币小数位
        /// </summary>
        [JsonProperty("tokenDecimal")]
        public int TokenDecimal { get; set; }
    }
}
