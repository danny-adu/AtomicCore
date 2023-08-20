using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Contract Log Json
    /// </summary>
    public class TronNetContractLogJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// topic
        /// </summary>
        [JsonProperty("topics")]
        public string[] Topics { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
