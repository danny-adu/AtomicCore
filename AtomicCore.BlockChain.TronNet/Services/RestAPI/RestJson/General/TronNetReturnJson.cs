using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Return Rest Json
    /// </summary>
    public class TronNetReturnJson
    {
        /// <summary>
        /// ContractRet
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }
    }
}
