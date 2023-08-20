using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc unspent output json
    /// </summary>
    public class BtcUnspentOutputJson
    {
        /// <summary>
        /// tx_hash_big_endian
        /// </summary>
        [JsonProperty("tx_hash_big_endian")]
        public string TxHashBigEndian { get; set; }

        /// <summary>
        /// tx_hash
        /// </summary>
        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// tx_output_n
        /// </summary>
        [JsonProperty("tx_output_n")]
        public int TxOutputN { get; set; }

        /// <summary>
        /// script
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// value_hex
        /// </summary>
        [JsonProperty("value_hex")]
        public string ValueHex { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// tx_index
        /// </summary>
        [JsonProperty("tx_index")]
        public ulong TxIndex { get; set; }
    }
}
