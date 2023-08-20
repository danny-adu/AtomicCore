using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Address Valid Rest Json
    /// </summary>
    public class TronNetAddressValidRestJson : TronNetValidRestJson
    {
        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
