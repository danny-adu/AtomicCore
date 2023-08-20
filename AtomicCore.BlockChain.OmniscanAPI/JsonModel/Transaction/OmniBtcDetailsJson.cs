using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni transaciton details json
    /// </summary>
    public class OmniBtcDetailsJson
    {
        /// <summary>
        /// tx hash
        /// </summary>
        [JsonProperty("hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// tx lock time
        /// </summary>
        [JsonProperty("locktime")]
        public ulong TxLockTime { get; set; }

        /// <summary>
        /// tx size
        /// </summary>
        [JsonProperty("size")]
        public int TxSize { get; set; }

        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string Txid { get; set; }

        /// <summary>
        /// tx verison
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// tx vins
        /// </summary>
        [JsonProperty("vin")]
        public OmniTxVinJson[] Vins { get; set; }

        /// <summary>
        /// tx vouts
        /// </summary>
        [JsonProperty("vout")]
        public OmniTxVoutJson[] Vouts { get; set; }

        /// <summary>
        /// vsize
        /// </summary>
        [JsonProperty("vsize")]
        public int VSize { get; set; }
    }
}
