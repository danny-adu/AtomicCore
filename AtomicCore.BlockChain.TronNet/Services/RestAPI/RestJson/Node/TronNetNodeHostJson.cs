using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node Host Json
    /// </summary>
    public class TronNetNodeHostJson
    {
        /// <summary>
        /// host
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        /// port
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }
    }
}
