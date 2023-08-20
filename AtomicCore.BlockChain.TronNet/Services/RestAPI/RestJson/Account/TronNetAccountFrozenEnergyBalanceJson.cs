using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Frozen Energy Balance
    /// </summary>
    public class TronNetAccountFrozenEnergyBalanceJson
    {
        /// <summary>
        /// frozen_balance
        /// </summary>
        [JsonProperty("frozen_balance")]
        public ulong FrozenBalance { get; set; }

        /// <summary>
        /// expire_time
        /// </summary>
        [JsonProperty("expire_time")]
        public ulong ExpireTime { get; set; }
    }
}
