using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Balance Change Json
    /// </summary>
    public class TronNetBlockBalanceJson : TronNetValidRestJson
    {
        /// <summary>
        /// block identifier
        /// </summary>
        [JsonProperty("block_identifier")]
        public TronNetBlockIdentifierJson BlockIdentifier { get; set; }

        /// <summary>
        /// timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// transaction_balance_trace
        /// </summary>
        [JsonProperty("transaction_balance_trace")]
        public TronNetTransactioBalanceTraceJson[] TransactioBalanceTraces { get; set; }
    }
}
