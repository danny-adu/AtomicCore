using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron TRC20 Transfer Json
    /// </summary>
    public class TronTRC20TransactionJson
    {
        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("transaction_id")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("block_ts"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// transfer form address
        /// </summary>
        [JsonProperty("from_address")]
        public string TransferFromAddress { get; set; }

        /// <summary>
        /// transfer to address
        /// </summary>
        [JsonProperty("to_address")]
        public string TransferToAddress { get; set; }

        /// <summary>
        /// contract address
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// quant amount
        /// </summary>
        [JsonProperty("quant")]
        public decimal Quant { get; set; }

        /// <summary>
        /// Approval Amount
        /// </summary>
        [JsonProperty("approval_amount")]
        public decimal ApprovalAmount { get; set; }

        /// <summary>
        /// event type
        /// </summary>
        [JsonProperty("event_type")]
        public string EventType { get; set; }

        /// <summary>
        /// Contract Type
        /// </summary>
        [JsonProperty("contract_type")]
        public string ContractType { get; set; }

        /// <summary>
        /// final result
        /// </summary>
        [JsonProperty("finalResult")]
        public string FinalResult { get; set; }

        /// <summary>
        /// contract return state
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// token info
        /// </summary>
        [JsonProperty("tokenInfo")]
        public TronTokenBasicJson TokenInfo { get; set; }

        /// <summary>
        /// from address is contract
        /// </summary>
        [JsonProperty("fromAddressIsContract")]
        public bool FromAddressIsContract { get; set; }

        /// <summary>
        /// to address is contract
        /// </summary>
        [JsonProperty("toAddressIsContract")]
        public bool ToAddressIsContract { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }

        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("blockId"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }
    }
}
