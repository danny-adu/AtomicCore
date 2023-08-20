using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Asset Issue Frozen Supply Json
    /// </summary>
    public class TronNetAssetIssueFrozenSupplyJson
    {
        /// <summary>
        /// frozen_amount
        /// </summary>
        [JsonProperty("frozen_amount")]
        public int FrozenAmount { get; set; }

        /// <summary>
        /// frozen_days
        /// </summary>
        [JsonProperty("frozen_days")]
        public int FrozenDays { get; set; }
    }
}
