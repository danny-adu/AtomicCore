using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Create Transaction RawData Rest Json
    /// </summary>
    public class TronNetCreateTransactionRawDataJson
    {
        /// <summary>
        /// contract
        /// </summary>
        [JsonProperty("contract")]
        public TronNetContractJson[] Contract { get; set; }

        /// <summary>
        /// ref_block_bytes
        /// </summary>
        [JsonProperty("ref_block_bytes")]
        public string RefBlockBytes { get; set; }

        /// <summary>
        /// ref_block_hash
        /// </summary>
        [JsonProperty("ref_block_hash")]
        public string RefBlockHash { get; set; }

        /// <summary>
        /// expiration
        /// </summary>
        [JsonProperty("expiration")]
        public ulong Expiration { get; set; }

        /// <summary>
        /// timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }
    }
}
