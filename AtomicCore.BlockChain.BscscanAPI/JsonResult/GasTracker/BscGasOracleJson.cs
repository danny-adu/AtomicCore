using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// Returns the current Safe, Proposed and Fast gas prices. 
    /// </summary>
    public class BscGasOracleJson
    {
        /// <summary>
        /// 最终的区块高度
        /// </summary>
        [JsonProperty("LastBlock")]
        public ulong LastBlock { get; set; }

        /// <summary>
        /// 最低手续费（安全手续费）
        /// </summary>
        [JsonProperty("SafeGasPrice")]
        public decimal SafeGasPrice { get; set; }

        /// <summary>
        /// 一般手续费
        /// </summary>
        [JsonProperty("ProposeGasPrice")]
        public decimal ProposeGasPrice { get; set; }

        /// <summary>
        /// 高手续费
        /// </summary>
        [JsonProperty("FastGasPrice")]
        public decimal FastGasPrice { get; set; }

        /// <summary>
        /// USD价值
        /// </summary>
        [JsonProperty("UsdPrice")]
        public decimal UsdPrice { get; set; }
    }
}
