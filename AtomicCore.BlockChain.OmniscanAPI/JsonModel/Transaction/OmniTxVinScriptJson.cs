using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni block vin script json
    /// </summary>
    public class OmniTxVinScriptJson
    {
        /// <summary>
        /// asm
        /// </summary>
        [JsonProperty("asm")]
        public string Asm { get; set; }

        /// <summary>
        /// hex
        /// </summary>
        [JsonProperty("hex")]
        public string Hex { get; set; }
    }
}
