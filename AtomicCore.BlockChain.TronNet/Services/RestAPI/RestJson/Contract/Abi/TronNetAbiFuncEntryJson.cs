using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Abi Function Entry Json
    /// </summary>
    public class TronNetAbiFuncEntryJson
    {
        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// constant
        /// </summary>
        [JsonProperty("constant")]
        public bool Constant { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// stateMutability
        /// </summary>
        [JsonProperty("stateMutability")]
        public string StateMutability { get; set; }

        /// <summary>
        /// inputs
        /// </summary>
        [JsonProperty("inputs")]
        public TronNetAbiFuncParamJson[] Inputs { get; set; }

        /// <summary>
        /// outputs
        /// </summary>
        [JsonProperty("outputs")]
        public TronNetAbiFuncReturnJson[] Outputs { get; set; }
    }
}
