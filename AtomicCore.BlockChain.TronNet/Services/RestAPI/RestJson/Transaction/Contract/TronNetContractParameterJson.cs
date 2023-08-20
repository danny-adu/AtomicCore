using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Contract Parameter Rest Json
    /// </summary>
    public class TronNetContractParameterJson
    {
        /// <summary>
        /// type_url
        /// </summary>
        [JsonProperty("type_url")]
        public string TypeUrl { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public JObject Value { get; set; }
    }
}
