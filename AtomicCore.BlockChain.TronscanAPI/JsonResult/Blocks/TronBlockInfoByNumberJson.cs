using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Block Info By Number Json
    /// </summary>
    public class TronBlockInfoByNumberJson : TronPageListJson
    {
        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronBlockDetailsJson[] Data { get; set; }
    }
}
