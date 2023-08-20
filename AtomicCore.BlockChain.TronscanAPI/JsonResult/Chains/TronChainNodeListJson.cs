using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Chain Node List Json
    /// </summary>
    public class TronChainNodeListJson
    {
        /// <summary>
        /// code
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// total
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronChainNodeJson[] Data { get; set; }
    }
}
