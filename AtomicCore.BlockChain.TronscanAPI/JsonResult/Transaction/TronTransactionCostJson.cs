using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Cotract Cost Json
    /// </summary>
    public class TronTransactionCostJson
    {
        /// <summary>
        /// NetFee
        /// </summary>
        [JsonProperty("net_fee")]
        public int NetFee { get; set; }

        /// <summary>
        /// EnergyUsage
        /// </summary>
        [JsonProperty("energy_usage")]
        public int EnergyUsage { get; set; }

        /// <summary>
        /// EnergyFee
        /// </summary>
        [JsonProperty("energy_fee")]
        public int EnergyFee { get; set; }

        /// <summary>
        /// EnergyUsageTotal
        /// </summary>
        [JsonProperty("energy_usage_total")]
        public int EnergyUsageTotal { get; set; }

        /// <summary>
        /// OriginEnergyUsage
        /// </summary>
        [JsonProperty("origin_energy_usage")]
        public int OriginEnergyUsage { get; set; }

        /// <summary>
        /// NetUsage
        /// </summary>
        [JsonProperty("net_usage")]
        public int NetUsage { get; set; }

        /// <summary>
        /// MultiSignFee
        /// </summary>
        [JsonProperty("multi_sign_fee")]
        public int MultiSignFee { get; set; }

        /// <summary>
        /// NetFeeCost
        /// </summary>
        [JsonProperty("net_fee_cost")]
        public int NetFeeCost { get; set; }

        /// <summary>
        /// EnergyFeeCost
        /// </summary>
        [JsonProperty("energy_fee_cost")]
        public int EnergyFeeCost { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("fee")]
        public int Fee { get; set; }
    }
}
