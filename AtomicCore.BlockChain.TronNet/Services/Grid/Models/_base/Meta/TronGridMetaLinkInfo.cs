using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Meta Link Info
    /// </summary>
    public class TronGridMetaLinkInfo
    {
        /// <summary>
        /// next page url
        /// </summary>
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
