using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni pager response
    /// </summary>
    public abstract class OmniPagerResponse
    {
        /// <summary>
        /// page total
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; set; }

        /// <summary>
        /// data total
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
