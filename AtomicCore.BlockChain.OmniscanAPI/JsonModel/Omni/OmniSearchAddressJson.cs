using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search address json
    /// </summary>
    public class OmniSearchAddressJson
    {
        /// <summary>
        /// balance
        /// </summary>
        [JsonProperty("balance")]
        public OmniSearchBalanceJson[] Balance { get; set; }
    }
}
