using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract ABI Code Json
    /// </summary>
    public class TronContractABICodeJson
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public TronApiStatusJson Status { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronContractMetadataJson Data { get; set; }
    }
}
