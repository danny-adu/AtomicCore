using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni coin list response
    /// </summary>
    public class OmniCoinListResponse
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// properties
        /// </summary>
        [JsonProperty("properties")]
        public OmniPropertyCoinBasicJson[] Properties { get; set; }
    }
}
