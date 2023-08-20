using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Trigger Info Json
    /// </summary>
    public class TronContractTriggerInfoJson
    {
        /// <summary>
        /// Method
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        [JsonProperty("parameter")]
        public TronContractTriggerParamJson Parameter { get; set; }

        /// <summary>
        /// ContractAddress
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// CallValue
        /// </summary>
        [JsonProperty("call_value")]
        public string CallValue { get; set; }
    }
}
