using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Header Raw Data Json
    /// </summary>
    public class TronNetBlockHeaderRawJson
    {
        /// <summary>
        /// block number
        /// </summary>
        [JsonProperty("number")]
        public ulong Number { get; set; }

        /// <summary>
        /// tx trieRoot
        /// </summary>
        [JsonProperty("txTrieRoot")]
        public string TxTrieRoot { get; set; }

        /// <summary>
        /// witness address
        /// </summary>
        [JsonProperty("witness_address")]
        public string WitnessAddress { get; set; }

        /// <summary>
        /// parentHash
        /// </summary>
        [JsonProperty("parentHash")]
        public string ParentHash { get; set; }

        /// <summary>
        /// timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }
    }
}
