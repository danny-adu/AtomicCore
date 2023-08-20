using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Tag Json
    /// </summary>
    public class TronContractTagJson
    {
        /// <summary>
        /// tag1
        /// </summary>
        [JsonProperty("tag1")]
        public string Tag1 { get; set; }

        /// <summary>
        /// tag1Url
        /// </summary>
        [JsonProperty("tag1Url")]
        public string Tag1Url { get; set; }

        /// <summary>
        /// vip
        /// </summary>
        [JsonProperty("vip")]
        public bool Vip { get; set; }
    }
}
