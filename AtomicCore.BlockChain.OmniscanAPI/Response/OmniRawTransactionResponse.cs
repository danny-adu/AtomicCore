using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni rawTransaction response
    /// </summary>
    public class OmniRawTransactionResponse
    {
        /// <summary>
        /// rawTransaction
        /// </summary>
        [JsonProperty("rawTransaction")]
        public string RawTransaction { get; set; }
    }
}
