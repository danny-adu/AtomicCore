using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni block vin json
    /// </summary>
    public class OmniTxVinJson
    {
        /// <summary>
        /// Txid
        /// </summary>
        [JsonProperty("txid")]
        public string Txid { get; set; }

        /// <summary>
        /// Sequence
        /// </summary>
        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// Vout count
        /// </summary>
        [JsonProperty("vout")]
        public int Vout { get; set; }

        /// <summary>
        /// ScriptSig
        /// </summary>
        [JsonProperty("scriptSig")]
        public OmniTxVinScriptJson ScriptSig { get; set; }
    }
}
