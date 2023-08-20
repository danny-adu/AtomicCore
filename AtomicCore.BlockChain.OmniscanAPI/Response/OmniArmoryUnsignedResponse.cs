using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni armory unsigned response
    /// </summary>
    public class OmniArmoryUnsignedResponse
    {
        /// <summary>
        /// armoryUnsigned
        /// </summary>
        [JsonProperty("armoryUnsigned")]
        public string ArmoryUnsigned { get; set; }
    }
}
