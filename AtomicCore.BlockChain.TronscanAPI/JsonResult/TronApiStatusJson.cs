using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Api Status Json
    /// </summary>
    public class TronApiStatusJson
    {
        /// <summary>
        /// Code
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
