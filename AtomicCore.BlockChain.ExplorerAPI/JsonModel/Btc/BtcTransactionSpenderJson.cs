using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc transaction spender json
    /// </summary>
    public class BtcTransactionSpenderJson
    {
        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string Txid { get; set; }

        /// <summary>
        /// input
        /// </summary>
        [JsonProperty("input")]
        public int Input { get; set; }
    }
}
