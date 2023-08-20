using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Chain Top Address Json
    /// </summary>
    public class TronChainTopAddressJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// address tag
        /// </summary>
        [JsonProperty("addressTag")]
        public string AddressTag { get; set; }

        /// <summary>
        /// address tag logo
        /// </summary>
        [JsonProperty("addressTagLogo")]
        public string AddressTagLogo { get; set; }

        /// <summary>
        /// balance
        /// </summary>
        [JsonProperty("balance")]
        public ulong Balance { get; set; }

        /// <summary>
        /// power
        /// </summary>
        [JsonProperty("power")]
        public ulong Power { get; set; }

        /// <summary>
        /// total transaction count
        /// </summary>
        [JsonProperty("totalTransactionCount")]
        public ulong TotalTransactionCount { get; set; }
    }
}
