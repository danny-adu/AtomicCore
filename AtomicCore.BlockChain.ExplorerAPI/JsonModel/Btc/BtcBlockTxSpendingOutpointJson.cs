using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc block tx spending outpoint json
    /// </summary>
    public class BtcBlockTxSpendingOutpointJson
    {
        /// <summary>
        /// spents
        /// </summary>
        [JsonProperty("tx_index")]
        public ulong TxIndex { get; set; }

        /// <summary>
        /// n
        /// </summary>
        [JsonProperty("n")]
        public int Txn { get; set; }
    }
}
