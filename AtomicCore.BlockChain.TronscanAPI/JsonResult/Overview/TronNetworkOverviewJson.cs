using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Network overview
    /// </summary>
    public class TronNetworkOverviewJson
    {
        /// <summary>
        /// 网络类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
