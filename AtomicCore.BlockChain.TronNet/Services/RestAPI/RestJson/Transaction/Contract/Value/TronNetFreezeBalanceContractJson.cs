using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Freeze Balance Contract Json
    /// </summary>
    public class TronNetFreezeBalanceContractJson : TronNetContractBaseValueJson
    {
        /// <summary>
        /// frozen_balance
        /// </summary>
        [JsonProperty("frozen_balance")]
        public ulong FrozenBalance { get; set; }

        /// <summary>
        /// frozen_duration
        /// </summary>
        [JsonProperty("frozen_duration")]
        public int FrozenDuration { get; set; }

        /// <summary>
        /// resource
        /// </summary>
        [JsonProperty("resource"),JsonConverter(typeof(TronNetResourceTypeJsonConverter))]
        public TronNetResourceType Resource { get; set; }
    }
}
