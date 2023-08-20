using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Contract Rest Json
    /// </summary>
    public class TronNetContractJson
    {
        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(TronNetContractTypeJsonConverter))]
        public TronNetContractType Type { get; set; }

        /// <summary>
        /// parameter
        /// </summary>
        [JsonProperty("parameter")]
        public TronNetContractParameterJson Parameter { get; set; }
    }
}
