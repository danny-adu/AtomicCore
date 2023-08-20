using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node Address Json
    /// </summary>
    public class TronNetNodeAddressJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public TronNetNodeHostJson Address { get; set; }
    }
}
