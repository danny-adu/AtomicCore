using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Token Call Json
    /// </summary>
    public class TronTokenCallJson
    {
        /// <summary>
        /// token id
        /// </summary>
        [JsonProperty("token_id")]
        public string TokenID { get; set; }

        /// <summary>
        /// call value
        /// </summary>
        [JsonProperty("call_value"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong CallValue { get; set; }

        /// <summary>
        /// Token Info
        /// </summary>
        [JsonProperty("tokenInfo")]
        public TronTokenBasicJson TokenInfo { get; set; }
    }
}
