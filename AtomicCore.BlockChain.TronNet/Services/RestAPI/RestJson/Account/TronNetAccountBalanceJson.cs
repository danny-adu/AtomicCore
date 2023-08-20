using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Balance Json
    /// </summary>
    public class TronNetAccountBalanceJson : TronNetValidRestJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// account_name
        /// </summary>
        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// trx balance
        /// </summary>
        [JsonProperty("balance"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal TrxBalance { get; set; }

        /// <summary>
        /// latest_opration_time
        /// </summary>
        [JsonProperty("latest_opration_time")]
        public ulong LatestOprationTime { get; set; }

        /// <summary>
        /// latest_consume_free_time
        /// </summary>
        [JsonProperty("latest_consume_free_time")]
        public ulong LatestConsumeFreeTime { get; set; }

        /// <summary>
        /// account_resource
        /// </summary>
        [JsonProperty("account_resource")]
        public TronNetAccountBalanceResourceJson AccountResource { get; set; }

        /// <summary>
        /// owner_permission
        /// </summary>
        [JsonProperty("owner_permission")]
        public TronNetAccountPermissionJson OwnerPermission { get; set; }

        /// <summary>
        /// active_permission
        /// </summary>
        [JsonProperty("active_permission")]
        public TronNetAccountOperatePermissionJson[] ActivePermission { get; set; }

        /// <summary>
        /// trc10 token asset list
        /// </summary>
        [JsonProperty("assetV2")]
        public TronNetAccountAssetV2Json[] AssetV2 { get; set; }
    }
}
