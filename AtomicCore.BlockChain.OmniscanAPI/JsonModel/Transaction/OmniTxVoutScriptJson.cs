using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni vout script json
    /// </summary>
    public class OmniTxVoutScriptJson
    {
        /// <summary>
        /// addresses
        /// </summary>
        [JsonProperty("addresses")]
        public string[] Addresses { get; set; }

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

        /// <summary>
        /// reqSigs
        /// </summary>
        [JsonProperty("reqSigs")]
        public int ReqSigs { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
