using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// Btc Single Block Response
    /// </summary>
    public class BtcSingleBlockResponse : ExplorerApiResponse
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("hash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// ver
        /// </summary>
        [JsonProperty("ver")]
        public int BlockVersion { get; set; }

        /// <summary>
        /// prev_block
        /// </summary>
        [JsonProperty("prev_block")]
        public string PrevBlockHash { get; set; }

        /// <summary>
        /// mrkl root
        /// </summary>
        [JsonProperty("mrkl_root")]
        public string BlockMrklRoot { get; set; }

        /// <summary>
        /// time
        /// </summary>
        [JsonProperty("time")]
        public ulong BlockTimpstamp { get; set; }

        /// <summary>
        /// bits
        /// </summary>
        [JsonProperty("bits")]
        public ulong BlockBits { get; set; }

        /// <summary>
        /// next_block
        /// </summary>
        [JsonProperty("next_block")]
        public string[] NextBlockHash { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("fee")]
        public ulong BlockFeeTotal { get; set; }

        /// <summary>
        /// nonce
        /// </summary>
        [JsonProperty("nonce")]
        public ulong BlockNonce { get; set; }

        /// <summary>
        /// n_tx
        /// </summary>
        [JsonProperty("n_tx")]
        public int BlockTxCount { get; set; }

        /// <summary>
        /// size
        /// </summary>
        [JsonProperty("size")]
        public int BlockSize { get; set; }

        /// <summary>
        /// block_index
        /// </summary>
        [JsonProperty("block_index")]
        public ulong BlockIndex { get; set; }

        /// <summary>
        /// main_chain
        /// </summary>
        [JsonProperty("main_chain")]
        public bool IsMainChain { get; set; }

        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("height")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// weight
        /// </summary>
        [JsonProperty("weight")]
        public int BlockWeight { get; set; }

        /////// <summary>
        /////// received_time
        /////// </summary>
        ////[JsonProperty("received_time")]
        ////public ulong ReceivedTime { get; set; }

        /////// <summary>
        /////// relayed_by
        /////// </summary>
        ////[JsonProperty("relayed_by")]
        ////public string RelayedBy { get; set; }

        /// <summary>
        /// tx
        /// </summary>
        [JsonProperty("tx")]
        public BtcBlockTransactionJson[] BlockTransactions { get; set; }
    }
}
