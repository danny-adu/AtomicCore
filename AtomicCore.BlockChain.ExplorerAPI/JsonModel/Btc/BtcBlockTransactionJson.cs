using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc block transaction json
    /// </summary>
    public class BtcBlockTransactionJson
    {
        /// <summary>
        /// transaction hash
        /// </summary>
        [JsonProperty("hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// ver
        /// </summary>
        [JsonProperty("ver")]
        public int TxVersion { get; set; }

        /// <summary>
        /// vin_sz
        /// </summary>
        [JsonProperty("vin_sz")]
        public int TxVinCount { get; set; }

        /// <summary>
        /// vout_sz
        /// </summary>
        [JsonProperty("vout_sz")]
        public int TxVoutCount { get; set; }

        /// <summary>
        /// size
        /// </summary>
        [JsonProperty("size")]
        public int TxSize { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("Fee")]
        public ulong TxFee { get; set; }

        /// <summary>
        /// relayed_by
        /// </summary>
        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        /// <summary>
        /// lock_time
        /// </summary>
        [JsonProperty("lock_time")]
        public ulong TxLockTime { get; set; }

        /// <summary>
        /// tx_index
        /// </summary>
        [JsonProperty("tx_index")]
        public ulong TxIndex { get; set; }

        /// <summary>
        /// double_spend
        /// </summary>
        [JsonProperty("double_spend")]
        public bool TxDoubleSpend { get; set; }

        /// <summary>
        /// times
        /// </summary>
        [JsonProperty("times")]
        public ulong TxTimestamp { get; set; }

        /// <summary>
        /// block_index
        /// </summary>
        [JsonProperty("block_index")]
        public ulong BlockIndex { get; set; }

        /// <summary>
        /// block_height
        /// </summary>
        [JsonProperty("block_height")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// tx vins
        /// </summary>
        [JsonProperty("inputs")]
        public BtcBlockTxVinJson[] TxVins { get; set; }

        /// <summary>
        /// tx out
        /// </summary>
        [JsonProperty("out")]
        public BtcBlockTxVoutJson[] TxVouts { get; set; }
    }
}
