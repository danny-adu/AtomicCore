using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Machine Memory Desc Info Json
    /// </summary>
    public class TronNetMachineMemoryDescInfoJson
    {
        /// <summary>
        /// initSize
        /// </summary>
        [JsonProperty("initSize")]
        public long InitSize { get; set; }

        /// <summary>
        /// maxSize
        /// </summary>
        [JsonProperty("maxSize")]
        public long MaxSize { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// useRate
        /// </summary>
        [JsonProperty("useRate")]
        public decimal UseRate { get; set; }

        /// <summary>
        /// useSize
        /// </summary>
        [JsonProperty("useSize")]
        public long UseSize { get; set; }
    }
}
