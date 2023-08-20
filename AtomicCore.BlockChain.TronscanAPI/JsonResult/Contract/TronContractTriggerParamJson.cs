using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Trigger Param Json
    /// </summary>
    public class TronContractTriggerParamJson
    {
        /// <summary>
        /// Number
        /// </summary>
        [JsonProperty("_number")]
        public string Number { get; set; }

        /// <summary>
        /// Direction
        /// </summary>
        [JsonProperty("_direction")]
        public string Direction { get; set; }
    }
}
