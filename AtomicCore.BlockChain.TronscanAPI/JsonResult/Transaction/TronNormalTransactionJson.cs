using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Last Transaction Json
    /// </summary>
    public class TronNormalTransactionJson
    {
        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// ownerAddress
        /// </summary>
        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// toAddress List
        /// </summary>
        [JsonProperty("toAddressList")]
        public string[] ToAddressList { get; set; }

        /// <summary>
        /// toAddress
        /// </summary>
        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// contractType
        /// </summary>
        [JsonProperty("contractType")]
        public TronscanContractType ContractType { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }

        /// <summary>
        /// Contract Data
        /// </summary>
        [JsonProperty("contractData")]
        public TronContractDataJson ContractData { get; set; }

        /// <summary>
        /// SmartCalls
        /// </summary>
        [JsonProperty("SmartCalls")]
        public string SmartCalls { get; set; }

        /// <summary>
        /// Events
        /// </summary>
        [JsonProperty("Events")]
        public string Events { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("fee")]
        public string Fee { get; set; }

        /// <summary>
        /// contract return state
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }

        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        /// <summary>
        /// amount
        /// trx transfer amount,unit is sun
        /// </summary>
        [JsonProperty("amount"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Amount { get; set; }

        /// <summary>
        /// cost
        /// </summary>
        [JsonProperty("cost")]
        public TronTransactionCostJson Cost { get; set; }

        /// <summary>
        /// tokenInfo
        /// </summary>
        [JsonProperty("tokenInfo")]
        public TronTokenBasicJson TokenInfo { get; set; }

        /// <summary>
        /// TokenType
        /// </summary>
        [JsonProperty("tokenType")]
        public string TokenType { get; set; }
    }
}
