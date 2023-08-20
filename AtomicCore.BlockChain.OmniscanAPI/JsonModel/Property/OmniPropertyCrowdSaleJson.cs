using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni property crowdsale json
    /// </summary>
    public class OmniPropertyCrowdSaleJson
    {
        /// <summary>
        /// active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// amountraised
        /// </summary>
        [JsonProperty("amountraised")]
        public decimal AmountRaised { get; set; }

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
        /// deadline
        /// </summary>
        [JsonProperty("deadline")]
        public ulong Deadline { get; set; }

        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

        /// <summary>
        /// earlybonus
        /// </summary>
        [JsonProperty("earlybonus")]
        public int Earlybonus { get; set; }

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
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// percenttoissuer
        /// </summary>
        [JsonProperty("percenttoissuer")]
        public string PercentToIssuer { get; set; }

        /// <summary>
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// propertyiddesired
        /// </summary>
        [JsonProperty("propertyiddesired")]
        public int PropertyIdDesired { get; set; }

        /// <summary>
        /// propertyiddesiredname
        /// </summary>
        [JsonProperty("propertyiddesiredname")]
        public string PropertyIdDesiredName { get; set; }

        /// <summary>
        /// starttime
        /// </summary>
        [JsonProperty("starttime")]
        public ulong StartTime { get; set; }

        /// <summary>
        /// subcategory
        /// </summary>
        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        /// <summary>
        /// tokensissued
        /// </summary>
        [JsonProperty("tokensissued")]
        public string TokensIssued { get; set; }

        /// <summary>
        /// tokensperunit
        /// </summary>
        [JsonProperty("tokensperunit")]
        public string Tokensperunit { get; set; }

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
