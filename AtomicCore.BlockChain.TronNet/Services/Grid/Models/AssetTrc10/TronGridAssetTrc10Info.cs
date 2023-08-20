using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Asset Trc10 Info
    /// </summary>
    public class TronGridAssetTrc10Info
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public int AssetID { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string AssetName { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [JsonProperty("description")]
        public string AssetDescription { get; set; }

        /// <summary>
        /// abbr
        /// </summary>
        [JsonProperty("abbr")]
        public string AssetAddress { get; set; }

        /// <summary>
        /// num
        /// </summary>
        public string AssetNum { get; set; }

        /// <summary>
        /// precision
        /// </summary>
        [JsonProperty("precision")]
        public int AssetPrecision { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [JsonProperty("url")]
        public string AssertUrl { get; set; }

        /// <summary>
        /// total_supply
        /// </summary>
        [JsonProperty("total_supply")]
        public System.Numerics.BigInteger TotalSupply { get; set; }

        /// <summary>
        /// trx_num
        /// </summary>
        [JsonProperty("trx_num")]
        public System.Numerics.BigInteger TrxNum { get; set; }

        /// <summary>
        /// vote_score
        /// </summary>
        [JsonProperty("vote_score")]
        public int VoteScore { get; set; }

        /// <summary>
        /// owner_address
        /// </summary>
        [JsonProperty("owner_address"),JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string OwnerAddress { get; set; }

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
    }
}
