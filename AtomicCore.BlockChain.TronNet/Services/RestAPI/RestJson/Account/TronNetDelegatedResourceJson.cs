using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Delegated Resource Json
    /// </summary>
    public class TronNetDelegatedResourceJson : TronNetValidRestJson
    {
        /// <summary>
        /// delegatedResource
        /// </summary>
        [JsonProperty("delegatedResource")]
        public TronNetDelegatedJson[] DelegatedResource { get; set; }
    }
}
