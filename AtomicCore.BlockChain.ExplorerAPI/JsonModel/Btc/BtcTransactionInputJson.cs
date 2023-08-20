using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc transaction input json
    /// </summary>
    public class BtcTransactionInputJson
    {
        /// <summary>
        /// coinbase
        /// </summary>
        [JsonProperty("coinbase")]
        public bool Coinbase { get; set; }

        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string Txid { get; set; }

        /// <summary>
        /// output
        /// </summary>
        [JsonProperty("output")]
        public int Output { get; set; }

        /// <summary>
        /// sigscript
        /// </summary>
        [JsonProperty("sigscript")]
        public string SigScript { get; set; }

        /// <summary>
        /// sequence
        /// </summary>
        [JsonProperty("sequence")]
        public ulong Sequence { get; set; }

        /// <summary>
        /// pkscript
        /// </summary>
        [JsonProperty("pkscript")]
        public string PkScript { get; set; }

        /// <summary>
        /// values
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// witness
        /// </summary>
        [JsonProperty("witness")]
        public string[] Witness { get; set; }
    }
}
