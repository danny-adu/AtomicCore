using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Chain Node Json
    /// </summary>
    public class TronChainNodeJson
    {
        /// <summary>
        /// country
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// lng
        /// </summary>
        [JsonProperty("lng")]
        public decimal Lng { get; set; }

        /// <summary>
        /// province
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// city
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// lat
        /// </summary>
        [JsonProperty("lat")]
        public decimal Lat { get; set; }
    }
}
