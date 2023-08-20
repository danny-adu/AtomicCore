using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Data Json
    /// </summary>
    public class TronContractDataJson
    {
        /// <summary>
        /// Owner Address
        /// </summary>
        [JsonProperty("owner_address")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// to address(only trx transfer exists)
        /// </summary>
        [JsonProperty("to_address")]
        public string ToAddress { get; set; }

        /// <summary>
        /// amount(only trx transfer exists)
        /// </summary>
        [JsonProperty("amount"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Amount { get; set; }

        /// <summary>
        /// Contract Address(only trc20 or trc721 exists)
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// Asset Name
        /// </summary>
        [JsonProperty("asset_name")]
        public string AssetName { get; set; }

        /// <summary>
        /// call value
        /// </summary>
        [JsonProperty("call_value"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong CallValue { get; set; }

        /// <summary>
        /// contract data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// tokenInfo
        /// </summary>
        [JsonProperty("tokenInfo")]
        public TronTokenBasicJson TokenInfo { get; set; }
    }
}
