using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Resource Transaction List Json
    /// </summary>
    public class TronResourceTransactionListJson : TronPageListJson
    {
        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronResourceTransactionJson[] Data { get; set; }
    }
}
