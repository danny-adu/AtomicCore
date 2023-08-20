using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni push transaction response
    /// </summary>
    public class OmniPushTxResponse
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// pushed
        /// </summary>
        [JsonProperty("pushed")]
        public string Pushed { get; set; }

        /// <summary>
        /// tx
        /// </summary>
        [JsonProperty("tx")]
        public string Txid { get; set; }
    }
}
