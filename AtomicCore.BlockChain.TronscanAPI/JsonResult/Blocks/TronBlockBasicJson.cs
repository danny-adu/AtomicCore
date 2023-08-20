using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Block Json Result
    /// </summary>
    public class TronBlockBasicJson
    {
        /// <summary>
        /// block hash
        /// </summary>
        [JsonProperty("hash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// 区块高度
        /// </summary>
        [JsonProperty("number"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// Block confirmed status
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// Block Size
        /// </summary>
        [JsonProperty("size"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Size { get; set; }

        /// <summary>
        /// Block Timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Parent Hash
        /// </summary>
        [JsonProperty("parentHash")]
        public string ParentHash { get; set; }

        /// <summary>
        /// witness address
        /// </summary>
        [JsonProperty("witnessAddress")]
        public string WitnessAddress { get; set; }

        /// <summary>
        /// nrOfTrx
        /// </summary>
        [JsonProperty("nrOfTrx")]
        public int NrOfTrx { get; set; }

        /// <summary>
        /// txTrieRoot
        /// </summary>
        [JsonProperty("txTrieRoot")]
        public string TxTrieRoot { get; set; }

        /// <summary>
        /// witnessId
        /// </summary>
        [JsonProperty("witnessId")]
        public int WitnessId { get; set; }
    }
}
