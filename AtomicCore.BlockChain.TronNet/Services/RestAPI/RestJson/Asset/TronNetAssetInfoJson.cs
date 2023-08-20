using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Asset Json
    /// </summary>
    public class TronNetAssetInfoJson : TronNetValidRestJson
    {
        /// <summary>
        /// Asset ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// owner_address
        /// </summary>
        [JsonProperty("owner_address")]
        public string OwnerAddress { get; set; }

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
        /// trx_num
        /// </summary>
        [JsonProperty("trx_num")]
        public ulong TrxNum { get; set; }

        /// <summary>
        /// precision
        /// </summary>
        [JsonProperty("precision")]
        public int Precision { get; set; }

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
    }
}
