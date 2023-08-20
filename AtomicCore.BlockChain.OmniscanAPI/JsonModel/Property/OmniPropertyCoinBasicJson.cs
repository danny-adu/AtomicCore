using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni property coin basic json
    /// </summary>
    public class OmniPropertyCoinBasicJson
    {
        /// <summary>
        /// blocktime
        /// </summary>
        [JsonProperty("blocktime")]
        public ulong BlockTimestamp { get; set; }

        /// <summary>
        /// category
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// creationtxid
        /// </summary>
        [JsonProperty("creationtxid")]
        public string CreationTxid { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

        /// <summary>
        /// fixedissuance
        /// </summary>
        [JsonProperty("fixedissuance")]
        public bool FixedIssuance { get; set; }

        /// <summary>
        /// issuer
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// managedissuance
        /// </summary>
        [JsonProperty("managedissuance")]
        public bool ManageDissuance { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// subcategory
        /// </summary>
        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        /// <summary>
        /// totaltokens
        /// </summary>
        [JsonProperty("totaltokens")]
        public decimal TotalTokens { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
