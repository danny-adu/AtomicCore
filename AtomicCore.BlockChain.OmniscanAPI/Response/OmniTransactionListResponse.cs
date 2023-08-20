using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni transaction list response
    /// </summary>
    public class OmniTransactionListResponse
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// pages
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; set; }

        /// <summary>
        /// transactions
        /// </summary>
        [JsonProperty("transactions")]
        public OmniTransactionJson[] Transaction { get; set; }
    }
}
