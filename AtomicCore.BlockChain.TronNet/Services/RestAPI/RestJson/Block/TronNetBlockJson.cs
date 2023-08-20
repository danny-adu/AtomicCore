using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Json
    /// </summary>
    public class TronNetBlockJson : TronNetValidRestJson
    {
        /// <summary>
        /// Block ID
        /// </summary>
        [JsonProperty("blockID")]
        public string BlockID { get; set; }

        /// <summary>
        /// block header
        /// </summary>
        [JsonProperty("block_header")]
        public TronNetBlockHeaderJson BlockHeader { get; set; }
    }
}
