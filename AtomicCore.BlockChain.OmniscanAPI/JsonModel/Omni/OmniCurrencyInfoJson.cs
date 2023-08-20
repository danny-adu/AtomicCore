using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni currency info json
    /// </summary>
    public class OmniCurrencyInfoJson
    {
        /// <summary>
        /// PropertyId
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// PropertyName
        /// </summary>
        [JsonProperty("propertyname")]
        public string PropertyName { get; set; }

        /// <summary>
        /// PropertyType
        /// </summary>
        [JsonProperty("propertytype")]
        public string PropertyType { get; set; }

        /// <summary>
        /// DisplayName
        /// </summary>
        [JsonProperty("displayname")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Property Type Int
        /// </summary>
        [JsonProperty("propertytype_int")]
        public int PropertyTypeInt { get; set; }
    }
}
