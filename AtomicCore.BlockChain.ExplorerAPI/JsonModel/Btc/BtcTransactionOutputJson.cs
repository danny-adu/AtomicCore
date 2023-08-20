using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc transaction output json
    /// </summary>
    public class BtcTransactionOutputJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// pkscript
        /// </summary>
        [JsonProperty("pkscript")]
        public string PkScript { get; set; }

        /// <summary>
        /// values
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// spent
        /// </summary>
        [JsonProperty("spent")]
        public string Spent { get; set; }

        /// <summary>
        /// spender
        /// </summary>
        [JsonProperty("spender")]
        public BtcTransactionSpenderJson Spender { get; set; }
    }
}
