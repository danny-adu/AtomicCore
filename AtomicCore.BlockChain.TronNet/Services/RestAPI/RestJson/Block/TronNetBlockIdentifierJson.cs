using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Identifier Json
    /// </summary>
    public class TronNetBlockIdentifierJson
    {
        /// <summary>
        /// Hash
        /// </summary>
        [JsonProperty("hash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// Number
        /// </summary>
        [JsonProperty("number")]
        public ulong Number { get; set; }
    }
}
