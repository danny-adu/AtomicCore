using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Normal Transfer Json
    /// </summary>
    public class TronNormalTransferJson
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// block
        /// </summary>
        [JsonProperty("block")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// transfer From Address
        /// </summary>
        [JsonProperty("transferFromAddress")]
        public string TransferFromAddress { get; set; }

        /// <summary>
        /// transfer To Address
        /// </summary>
        [JsonProperty("transferToAddress")]
        public string TransferToAddress { get; set; }

        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// tokenName
        /// </summary>
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// contract return state
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }

        /// <summary>
        /// tokenInfo
        /// </summary>
        [JsonProperty("tokenInfo")]
        public TronTokenBasicJson TokenInfo { get; set; }
    }
}
