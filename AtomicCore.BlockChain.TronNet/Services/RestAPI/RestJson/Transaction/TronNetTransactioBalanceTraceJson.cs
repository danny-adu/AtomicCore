using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transactio Balance Trace
    /// </summary>
    public class TronNetTransactioBalanceTraceJson
    {
        /// <summary>
        /// transaction identifier
        /// </summary>
        [JsonProperty("transaction_identifier")]
        public string TransactionIdentifier { get; set; }

        /// <summary>
        /// operations
        /// </summary>
        [JsonProperty("operation")]
        public TronNetTransactioBalanceOperationJson[] Operations { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(TronNetContractTypeJsonConverter))]
        public TronNetContractType Type { get; set; }

        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status"), JsonConverter(typeof(TronNetTransactionReciptStatusJsonConverter))]
        public TronNetTransactionReciptStatus Status { get; set; }
    }
}
