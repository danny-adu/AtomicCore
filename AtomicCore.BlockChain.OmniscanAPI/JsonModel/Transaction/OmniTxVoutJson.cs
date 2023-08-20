using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni block vout json
    /// </summary>
    public class OmniTxVoutJson
    {
        /// <summary>
        /// n
        /// </summary>
        [JsonProperty("n")]
        public int N { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public decimal Value { get; set; }

        /// <summary>
        /// scriptPubKey
        /// </summary>
        [JsonProperty("scriptPubKey")]
        public OmniTxVoutScriptJson ScriptPubKey { get; set; }
    }
}
