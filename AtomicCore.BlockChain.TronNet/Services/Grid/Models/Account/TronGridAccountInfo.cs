using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Account Info
    /// </summary>
    public class TronGridAccountInfo
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string Address { get; set; }

        /// <summary>
        /// trx balance
        /// </summary>
        [JsonProperty("balance"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal Balance { get; set; }

        /// <summary>
        /// owner_permission
        /// </summary>
        [JsonProperty("owner_permission")]
        public TronGridPermissionInfo OwnerPermission { get; set; }

        /// <summary>
        /// active_permission
        /// </summary>
        [JsonProperty("active_permission")]
        public TronGridActivePermissionInfo[] ActivePermission { get; set; }

        /// <summary>
        /// create time # UTC-TIMESTAMP
        /// </summary>
        [JsonProperty("create_time")]
        public ulong CreateTime { get; set; }

        /// <summary>
        /// latest opration time
        /// </summary>
        [JsonProperty("latest_opration_time")]
        public ulong LatestOprationTime { get; set; }

        /// <summary>
        /// trc10 # trc10 asset list
        /// </summary>
        [JsonProperty("assetV2")]
        public TronGridKVInfo[] TRC10 { get; set; }

        /// <summary>
        /// trc20 # trc20 asset list
        /// </summary>
        [JsonProperty("trc20"), JsonConverter(typeof(TronGridTRC20KVJsonConverter))]
        public Dictionary<string, System.Numerics.BigInteger> TRC20 { get; set; }

        ///////// <summary>
        ///////// free_asset_net_usageV2
        ///////// </summary>
        //////[JsonProperty("free_asset_net_usageV2")]
        //////public TronGridKVInfo[] FreeAssetNetUsageV2 { get; set; }

        /////// <summary>
        /////// frozen self trx balance for self get bandwidth
        /////// </summary>
        ////[JsonProperty("frozen")]
        ////public TronGridFrozenInfo[] BandwidthResource { get; set; }

        /////// <summary>
        /////// account resource # energy
        /////// </summary>
        ////[JsonProperty("account_resource")]
        ////public TronGridEnergyResource AccountResource { get; set; }

        /////// <summary>
        /////// latest consume free time # UTC-TIMESTAMP
        /////// </summary>
        ////[JsonProperty("latest_consume_free_time")]
        ////public ulong LatestConsumeFreeTime { get; set; }

        /////// <summary>
        /////// delegated_frozen_balance_for_bandwidth # Fronze TRX FOR OTHER ADDRESS
        /////// </summary>
        ////[JsonProperty("delegated_frozen_balance_for_bandwidth"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        ////public decimal DelegatedFrozenBalanceForBandwidth { get; set; }
    }
}
