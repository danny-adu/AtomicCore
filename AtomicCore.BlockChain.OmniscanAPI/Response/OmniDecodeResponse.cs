using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni decode response
    /// </summary>
    public class OmniDecodeResponse
    {
        /// <summary>
        /// BTC
        /// </summary>
        [JsonProperty("BTC")]
        public KeyValuePair<string, OmniBtcDetailsJson> BTC { get; set; }

        /// <summary>
        /// OMNI
        /// </summary>
        [JsonProperty("OMNI")]
        public KeyValuePair<string, OmniPropertyBasicJson> OMNI { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        [JsonProperty("Reference")]
        public string Reference { get; set; }

        /// <summary>
        /// Sender
        /// </summary>
        [JsonProperty("Sender")]
        public string Sender { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Inputs
        /// </summary>
        [JsonProperty("inputs")]
        public Dictionary<string, decimal> Inputs { get; set; }
    }
}
