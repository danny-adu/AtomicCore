using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Abi Function Return Json
    /// </summary>
    public class TronNetAbiFuncReturnJson
    {
        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
