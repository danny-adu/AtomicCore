using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transaction Recipt Json
    /// </summary>
    public class TronNetTransactionReciptJson
    {
        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result"),JsonConverter(typeof(TronNetTransactionReciptStatusJsonConverter))]
        public TronNetTransactionReciptStatus Result { get; set; }

        /// <summary>
        /// energy fee
        /// </summary>
        [JsonProperty("energy_fee")]
        public ulong EnergyFee { get; set; }

        /// <summary>
        /// energy usage total
        /// </summary>
        [JsonProperty("energy_usage_total")]
        public ulong EnergyUsageTotal { get; set; }

        /// <summary>
        /// net usage
        /// </summary>
        [JsonProperty("net_usage")]
        public ulong NetUsage { get; set; }
    }
}
