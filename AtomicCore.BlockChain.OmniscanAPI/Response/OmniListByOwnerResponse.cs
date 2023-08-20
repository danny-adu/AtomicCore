using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni list by owner resposne
    /// </summary>
    public class OmniListByOwnerResponse
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// properties
        /// </summary>
        [JsonProperty("properties")]
        public OmniPropertyInfoExtJson[] Properties { get; set; }
    }
}
