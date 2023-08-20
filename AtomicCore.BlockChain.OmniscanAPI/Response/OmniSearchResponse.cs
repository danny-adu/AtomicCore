using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search response
    /// </summary>
    public class OmniSearchResponse
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// query
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public OmniSearchCombinedJson Data { get; set; }
    }
}
