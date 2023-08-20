using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Energy Resource
    /// </summary>
    public class TronGridEnergyResource
    {
        /// <summary>
        /// frozen self trx balance for self get energy
        /// </summary>
        [JsonProperty("frozen_balance_for_energy")]
        public TronGridFrozenInfo FrozenBalanceForEnergy { get; set; }

        /// <summary>
        /// frozen self trx for other address get energy
        /// </summary>
        [JsonProperty("delegated_frozen_balance_for_energy"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal DelegatedFrozenBalanceForEnergy { get; set; }

        /// <summary>
        /// latest consume time for energy
        /// </summary>
        [JsonProperty("latest_consume_time_for_energy")]
        public ulong LatestConsumeTimeForEnergy { get; set; }
    }
}
