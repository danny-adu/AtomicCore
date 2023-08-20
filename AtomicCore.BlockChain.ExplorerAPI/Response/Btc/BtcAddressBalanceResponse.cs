using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// the btc balance of address
    /// </summary>
    public class BtcAddressBalanceResponse : ExplorerApiResponse
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public int Confirmed { get; set; }

        /// <summary>
        /// unconfirmed
        /// </summary>
        [JsonProperty("unconfirmed")]
        public int Unconfirmed { get; set; }

        /// <summary>
        /// utxo
        /// </summary>
        [JsonProperty("utxo")]
        public int Utxo { get; set; }

        /// <summary>
        /// txs
        /// </summary>
        [JsonProperty("txs")]
        public int Txs { get; set; }

        /// <summary>
        /// received
        /// </summary>
        [JsonProperty("received")]
        public ulong Received { get; set; }
    }
}
