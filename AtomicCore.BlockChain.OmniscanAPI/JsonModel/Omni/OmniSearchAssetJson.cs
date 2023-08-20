using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni search asset json
    /// </summary>
    public class OmniSearchAssetJson
    {
        /// <summary>
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// propertyname
        /// </summary>
        [JsonProperty("propertyname")]
        public string PropertyName { get; set; }

        /// <summary>
        /// referenceaddress
        /// </summary>
        [JsonProperty("referenceaddress")]
        public string ReferenceAddress { get; set; }
    }
}
