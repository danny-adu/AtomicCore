using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni asset property info json
    /// </summary>
    public class OmniAssetBalanceJson
    {
        /// <summary>
        /// asset id
        /// </summary>
        [JsonProperty("id")]
        public int AssetID { get; set; }

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

        /// <summary>
        /// Asset frozen
        /// </summary>
        [JsonProperty("frozen")]
        public decimal AssetFrozen { get; set; }

        /// <summary>
        /// Asset Reserved
        /// </summary>
        [JsonProperty("reserved")]
        public decimal AssetReserved { get; set; }

        /// <summary>
        /// Asset Divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool AssetDivisible { get; set; }

        /// <summary>
        /// Asset Pendingpos
        /// </summary>
        [JsonProperty("pendingpos")]
        public string AssetPendingpos { get; set; }

        /// <summary>
        /// Asset Pendingneg
        /// </summary>
        [JsonProperty("pendingneg")]
        public string AssetPendingneg { get; set; }

        /// <summary>
        /// propertyinfo
        /// </summary>
        [JsonProperty("propertyinfo")]
        public OmniPropertyInfoJson PropertyInfo { get; set; }

        /// <summary>
        /// Asset Error
        /// </summary>
        [JsonProperty("error")]
        public bool AssetError { get; set; }
    }
}
