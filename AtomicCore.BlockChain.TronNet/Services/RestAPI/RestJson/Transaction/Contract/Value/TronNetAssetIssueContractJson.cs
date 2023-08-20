using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Asset Issue Caontract Json
    /// </summary>
    public class TronNetAssetIssueContractJson : TronNetContractBaseValueJson
    {
        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// abbr
        /// </summary>
        [JsonProperty("abbr")]
        public string Abbr { get; set; }

        /// <summary>
        /// total_supply
        /// </summary>
        [JsonProperty("total_supply")]
        public ulong TotalSupply { get; set; }

        /// <summary>
        /// precision
        /// </summary>
        [JsonProperty("precision")]
        public int Precision { get; set; }

        /// <summary>
        /// trx_num
        /// </summary>
        [JsonProperty("trx_num")]
        public virtual ulong TrxNum { get; set; }

        /// <summary>
        /// num
        /// </summary>
        [JsonProperty("num")]
        public ulong Num { get; set; }

        /// <summary>
        /// start_time
        /// </summary>
        [JsonProperty("start_time")]
        public ulong StartTime { get; set; }

        /// <summary>
        /// end_time
        /// </summary>
        [JsonProperty("end_time")]
        public ulong EndTime { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// free_asset_net_limit
        /// </summary>
        [JsonProperty("free_asset_net_limit")]
        public ulong FreeAssetNetLimit { get; set; }

        /// <summary>
        /// public_free_asset_net_limit
        /// </summary>
        [JsonProperty("public_free_asset_net_limit")]
        public ulong PublicFreeAssetNetLimit { get; set; }

        /// <summary>
        /// frozen_supply
        /// </summary>
        [JsonProperty("frozen_supply")]
        public TronNetAssetIssueFrozenSupplyJson FrozenSupply { get; set; }
    }
}
