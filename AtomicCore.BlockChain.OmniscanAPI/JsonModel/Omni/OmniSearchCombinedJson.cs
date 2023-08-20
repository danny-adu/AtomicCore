using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search data json
    /// </summary>
    public class OmniSearchCombinedJson
    {
        /// <summary>
        /// address result
        /// </summary>
        [JsonProperty("address")]
        public OmniSearchAddressJson Address { get; set; }

        /// <summary>
        /// asset
        /// </summary>
        [JsonProperty("asset")]
        [JsonConverter(typeof(OmniSearchAssetJsonConvert))]
        public OmniSearchAssetJson[] Asset { get; set; }

        /// <summary>
        /// tx
        /// </summary>
        [JsonProperty("tx")]
        public OmniTransactionJson Transaction { get; set; }
    }
}
