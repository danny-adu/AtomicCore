using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc unspent output response
    /// </summary>
    public class BtcUnspentOutputResponse : ExplorerApiResponse
    {
        /// <summary>
        /// notice
        /// </summary>
        [JsonProperty("notice")]
        public string Notice { get; set; }

        /// <summary>
        /// unspent_outputs
        /// </summary>
        [JsonProperty("unspent_outputs")]
        public BtcUnspentOutputJson[] UnspentOutputs { get; set; }
    }
}
