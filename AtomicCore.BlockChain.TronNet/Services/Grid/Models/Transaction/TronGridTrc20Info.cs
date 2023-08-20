using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Trc20 Info
    /// </summary>
    public class TronGridTrc20Info
    {
        /// <summary>
        /// txID
        /// </summary>
        [JsonProperty("transaction_id")]
        public string TxHash { get; set; }

        /// <summary>
        /// from
        /// </summary>
        [JsonProperty("from")]
        public string TxFromAddress { get; set; }

        /// <summary>
        /// to
        /// </summary>
        [JsonProperty("to")]
        public string TxToAddress { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string TxType { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public System.Numerics.BigInteger TxValue { get; set; }

        /// <summary>
        /// block_timestamp
        /// </summary>
        [JsonProperty("block_timestamp")]
        public long BlockTimestamp { get; set; }

        /// <summary>
        /// token_info
        /// </summary>
        [JsonProperty("token_info")]
        public TronGridContractInfo TokenInfo { get; set; }
    }
}
