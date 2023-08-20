using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc block transaction vin json
    /// </summary>
    public class BtcBlockTxVinJson
    {
        /// <summary>
        /// sequence
        /// </summary>
        [JsonProperty("sequence")]
        public ulong Sequence { get; set; }

        /// <summary>
        /// witness
        /// </summary>
        [JsonProperty("witness")]
        public string[] Witness { get; set; }

        /// <summary>
        /// sigscript
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// output
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// prev_out
        /// </summary>
        [JsonProperty("prev_out")]
        public BtcBlockTxVinPrevoutJson Prevout { get; set; }
    }
}
