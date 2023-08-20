using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Page List Json
    /// </summary>
    public abstract class TronPageListJson
    {
        /// <summary>
        /// page limit
        /// </summary>
        [JsonProperty("total")]
        public int PageSize { get; set; }

        /// <summary>
        /// range total count
        /// </summary>
        [JsonProperty("rangeTotal")]
        public int TotalCount { get; set; }
    }
}
