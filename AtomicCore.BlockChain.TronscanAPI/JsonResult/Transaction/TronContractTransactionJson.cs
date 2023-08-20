using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Transaction Json
    /// </summary>
    public class TronContractTransactionJson
    {
        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// call data
        /// </summary>
        [JsonProperty("call_data")]
        public string CallData { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// ownAddress
        /// </summary>
        [JsonProperty("ownAddress")]
        public string OwnAddress { get; set; }

        /// <summary>
        /// ownAddressType
        /// </summary>
        [JsonProperty("ownAddressType")]
        public string OwnAddressType { get; set; }

        /// <summary>
        /// parentHash
        /// </summary>
        [JsonProperty("parentHash")]
        public string ParentHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Value { get; set; }

        /// <summary>
        /// toAddress
        /// </summary>
        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// toAddressType
        /// </summary>
        [JsonProperty("toAddressType")]
        public string ToAddressType { get; set; }

        /// <summary>
        /// token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// tx fee
        /// </summary>
        [JsonProperty("txFee")]
        public ulong TxFee { get; set; }

        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("txHash")]
        public string TransactionHash { get; set; }

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
        /// cost
        /// </summary>
        [JsonProperty("cost")]
        public TronTransactionCostJson Cost { get; set; }
    }
}
