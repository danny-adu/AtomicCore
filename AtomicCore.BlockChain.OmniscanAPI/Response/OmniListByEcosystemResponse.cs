using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni list by ecos system response
    /// </summary>
    public class OmniListByEcosystemResponse
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
