using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc last price json
    /// </summary>
    public class BscLastPriceJson
    {
        /// <summary>
        /// ethbtc
        /// </summary>
        [JsonProperty("ethbtc")]
        public decimal Ethbtc { get; set; }

        /// <summary>
        /// ethbtc_timestamp
        /// </summary>
        [JsonProperty("ethbtc_timestamp")]
        public long EthbtcTimestamp { get; set; }

        /// <summary>
        /// ethusd
        /// </summary>
        [JsonProperty("ethusd")]
        public decimal Ethusd { get; set; }

        /// <summary>
        /// ethusd_timestamp
        /// </summary>
        [JsonProperty("ethusd_timestamp")]
        public long EthusdTimestamp { get; set; }
    }
}
