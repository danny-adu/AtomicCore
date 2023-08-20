using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Unfreeze Balance Contract Json
    /// </summary>
    public class TronNetUnfreezeBalanceContractJson : TronNetContractBaseValueJson
    {
        /// <summary>
        /// resource
        /// </summary>
        [JsonProperty("resource"), JsonConverter(typeof(TronNetResourceTypeJsonConverter))]
        public TronNetResourceType Resource { get; set; }
    }
}
