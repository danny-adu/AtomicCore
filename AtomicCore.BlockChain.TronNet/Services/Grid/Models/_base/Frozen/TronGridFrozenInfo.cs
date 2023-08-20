using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Frozen Info
    /// </summary>
    public class TronGridFrozenInfo
    {
        /// <summary>
        /// frozen_balance # TRX COST
        /// </summary>
        [JsonProperty("frozen_balance"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal FrozenBalance { get; set; }

        /// <summary>
        /// expire_time # UTC-TIMESTAMP
        /// </summary>
        [JsonProperty("expire_time")]
        public ulong ExpireTime { get; set; }
    }
}
