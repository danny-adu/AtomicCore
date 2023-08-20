using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search balance json
    /// </summary>
    public class OmniSearchBalanceJson
    {
        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

        /// <summary>
        /// frozen
        /// </summary>
        [JsonProperty("frozen")]
        public decimal Frozen { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }

        /// <summary>
        /// Asset Pendingneg
        /// </summary>
        [JsonProperty("pendingneg")]
        public string AssetPendingneg { get; set; }

        /// <summary>
        /// Asset Pendingpos
        /// </summary>
        [JsonProperty("pendingpos")]
        public string AssetPendingpos { get; set; }

        /// <summary>
        /// propertyinfo
        /// </summary>
        [JsonProperty("propertyinfo")]
        public JObject propertyinfo { get; set; }

        /// <summary>
        /// Asset Reserved
        /// </summary>
        [JsonProperty("reserved")]
        public decimal AssetReserved { get; set; }

        /// <summary>
        /// asset symbol
        /// </summary>
        [JsonProperty("symbol")]
        public string AssetSymbol { get; set; }

        /// <summary>
        /// asset value
        /// </summary>
        [JsonProperty("value")]
        public decimal AssetValue { get; set; }
    }
}
