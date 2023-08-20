using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node Json
    /// </summary>
    public class TronNetNodeJson : TronNetValidRestJson
    {
        /// <summary>
        /// nodes
        /// </summary>
        [JsonProperty("nodes")]
        public TronNetNodeAddressJson[] Nodes { get; set; }
    }
}
