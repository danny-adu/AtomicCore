using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// 获取当前手续费标准（三档）
    /// https://cn.etherscan.com/apis#gastracker
    /// Get Gas Oracle
    /// </summary>
    public class EthGasOracleJsonResult
    {
        /// <summary>
        /// 区块高度
        /// </summary>
        [JsonProperty("LastBlocks")]
        public int LastBlocks { get; set; }

        /// <summary>
        /// 最低
        /// </summary>
        [JsonProperty("SafeGasPrice")]
        public string SafeGasPrice { get; set; }

        /// <summary>
        /// 中等
        /// </summary>
        [JsonProperty("ProposeGasPrice")]
        public string ProposeGasPrice { get; set; }

        /// <summary>
        /// 最快
        /// </summary>
        [JsonProperty("FastGasPrice")]
        public string FastGasPrice { get; set; }
    }
}
