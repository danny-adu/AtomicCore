using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Block Info List Json
    /// </summary>
    public class TronBlockInfoListJson : TronPageListJson
    {
        /// <summary>
        /// block info list
        /// </summary>
        [JsonProperty("data")]
        public TronBlockInfoJson[] Data { get; set; }
    }
}
