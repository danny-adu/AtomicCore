using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni crowd sales response
    /// </summary>
    public class OmniCrowdSalesResponse
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// CrowSales
        /// </summary>
        [JsonProperty("crowdsales")]
        public OmniPropertyCrowdSaleJson[] CrowSales { get; set; }
    }
}
