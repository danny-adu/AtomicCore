using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron SR Confirm Json
    /// </summary>
    public class TronSRConfirmJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// block
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
