using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc rpc transaction receipt log json
    /// </summary>
    public class BscRpcTransactionReceiptLogJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// topics
        /// </summary>
        [JsonProperty("topics")]
        public string[] Topics { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// hash
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// hash
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TxHash { get; set; }

        /// <summary>
        /// transactionIndex
        /// </summary>
        [JsonProperty("transactionIndex"), JsonConverter(typeof(BscHexInt32JsonConverter))]
        public int TransactionIndex { get; set; }

        /// <summary>
        /// number
        /// </summary>
        [JsonProperty("blockNumber"), JsonConverter(typeof(BscHexInt64JsonConverter))]
        public long BlockNumber { get; set; }

        /// <summary>
        /// logIndex
        /// </summary>
        [JsonProperty("logIndex"), JsonConverter(typeof(BscHexInt32JsonConverter))]
        public int LogIndex { get; set; }

        /// <summary>
        /// removed
        /// </summary>
        [JsonProperty("removed")]
        public bool Removed { get; set; }
    }
}
