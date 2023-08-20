using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Fronzen Supply Json
    /// </summary>
    public class TronNetFrozenSupplyJson
    {
        /// <summary>
        /// frozen amount
        /// </summary>
        [JsonProperty("frozen_amount")]
        public ulong FrozenAmount { get; set; }

        /// <summary>
        /// frozen days
        /// </summary>
        [JsonProperty("frozen_days")]
        public int FrozenDays { get; set; }
    }
}
