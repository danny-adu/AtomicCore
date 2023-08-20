using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc rpc transaction receipt json
    /// </summary>
    public class BscRpcTransactionReceiptJson
    {
        /// <summary>
        /// hash
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// number
        /// </summary>
        [JsonProperty("blockNumber"), JsonConverter(typeof(BscHexInt64JsonConverter))]
        public long BlockNumber { get; set; }

        /// <summary>
        /// contractAddress
        /// </summary>
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// cumulativeGasUsed
        /// </summary>
        [JsonProperty("cumulativeGasUsed"), JsonConverter(typeof(BscHexInt64JsonConverter))]
        public long CumulativeGasUsed { get; set; }

        /// <summary>
        /// from
        /// </summary>
        [JsonProperty("from")]
        public string TxFrom { get; set; }

        /// <summary>
        /// gasUsed
        /// </summary>
        [JsonProperty("gasUsed"), JsonConverter(typeof(BscHexInt64JsonConverter))]
        public long TxGasUsed { get; set; }

        /// <summary>
        /// logs
        /// </summary>
        [JsonProperty("logs")]
        public BscRpcTransactionReceiptLogJson[] Logs { get; set; }

        /// <summary>
        /// logsBloom
        /// </summary>
        [JsonProperty("logsBloom")]
        public string LogsBloom { get; set; }

        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status"),JsonConverter(typeof(BscTxReceiptStatusConverter))]
        public BscReceiptStatus TxStatus { get; set; }

        /// <summary>
        /// to
        /// </summary>
        [JsonProperty("to")]
        public string TxTo { get; set; }

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
        /// type
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(BscHexInt32JsonConverter))]
        public int Type { get; set; }
    }
}
