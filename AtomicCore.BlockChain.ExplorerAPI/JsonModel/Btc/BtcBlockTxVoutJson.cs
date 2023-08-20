using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// Btc Block TxVout Json
    /// </summary>
    public class BtcBlockTxVoutJson
    {
        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// spent
        /// </summary>
        [JsonProperty("spent")]
        public bool Spent { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// spending_outpoints
        /// </summary>
        [JsonProperty("spending_outpoints")]
        public BtcBlockTxSpendingOutpointJson[] SpendingOutpoints { get; set; }

        /// <summary>
        /// n
        /// </summary>
        [JsonProperty("n")]
        public int N { get; set; }

        /// <summary>
        /// tx_index
        /// </summary>
        [JsonProperty("tx_index")]
        public ulong TxIndex { get; set; }

        /// <summary>
        /// script
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// addr
        /// </summary>
        [JsonProperty("addr")]
        public string Address { get; set; }
    }
}
