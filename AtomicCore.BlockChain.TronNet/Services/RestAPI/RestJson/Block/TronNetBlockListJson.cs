using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block List Json
    /// </summary>
    public class TronNetBlockListJson : TronNetValidRestJson
    {
        /// <summary>
        /// block list
        /// </summary>
        [JsonProperty("block")]
        public TronNetBlockJson[] Blocks { get; set; }
    }
}
