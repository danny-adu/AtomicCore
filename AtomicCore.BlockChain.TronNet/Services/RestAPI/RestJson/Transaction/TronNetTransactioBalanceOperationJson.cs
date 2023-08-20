using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transaction Balance Change Json
    /// </summary>
    public class TronNetTransactioBalanceOperationJson
    {
        /// <summary>
        /// operation_identifier
        /// </summary>
        [JsonProperty("operation_identifier")]
        public int OperationIdentifier { get; set; }

        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
