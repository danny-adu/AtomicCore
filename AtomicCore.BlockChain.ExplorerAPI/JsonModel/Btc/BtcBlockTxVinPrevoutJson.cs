using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc block tx vin prevout json
    /// </summary>
    public class BtcBlockTxVinPrevoutJson
    {
        /// <summary>
        /// spents
        /// </summary>
        [JsonProperty("spents")]
        public bool Spents { get; set; }

        /// <summary>
        /// script
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// spending_outpoints
        /// </summary>
        [JsonProperty("spending_outpoints")]
        public BtcBlockTxSpendingOutpointJson[] SpendingOutpoints { get; set; }

        /// <summary>
        /// tx_index
        /// </summary>
        [JsonProperty("tx_index")]
        public ulong TxIndex { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// addr
        /// </summary>
        [JsonProperty("addr")]
        public string Address { get; set; }

        /// <summary>
        /// n
        /// </summary>
        [JsonProperty("n")]
        public int N { get; set; }

        /// <summary>
        /// types
        /// </summary>
        [JsonProperty("types")]
        public int Types { get; set; }
    }
}
