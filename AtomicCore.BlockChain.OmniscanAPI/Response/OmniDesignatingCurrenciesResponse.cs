using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni designating currencies response
    /// </summary>
    public class OmniDesignatingCurrenciesResponse
    {
        /// <summary>
        /// Currencies
        /// </summary>
        [JsonProperty("currencies")]
        public OmniCurrencyInfoJson[] Currencies { get; set; }

        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        /// filter
        /// </summary>
        [JsonProperty("filter")]
        public long[] Filter { get; set; }
    }
}
