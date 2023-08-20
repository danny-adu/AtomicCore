using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc transaction receipt status json
    /// </summary>
    public class BscTransactionReceiptStatusJson
    {
        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public BscReceiptStatus Status { get; set; }
    }
}
