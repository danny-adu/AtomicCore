using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Internal Transfer Json
    /// </summary>
    public class TronInternalTransactionJson
    {
        /// <summary>
        /// Transaction Hash
        /// </summary>
        [JsonProperty("hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// form address
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// to address
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// rejected
        /// </summary>
        [JsonProperty("rejected")]
        public bool Rejected { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }

        /// <summary>
        /// note
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }

        /// <summary>
        /// token_list
        /// </summary>
        [JsonProperty("token_list")]
        public TronTokenCallJson TokenList { get; set; }

        /// <summary>
        /// Value Info List
        /// </summary>
        [JsonProperty("valueInfoList")]
        public TronTokenValueJson[] ValueInfoList { get; set; }

        /// <summary>
        /// Token ID
        /// </summary>
        [JsonProperty("token_id")]
        public string TokenID { get; set; }

        /// <summary>
        /// Call Value
        /// </summary>
        [JsonProperty("call_value"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong CallValue { get; set; }

        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }
    }
}
