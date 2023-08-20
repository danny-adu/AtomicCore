using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron TRC20 Transfer Event Json
    /// </summary>
    public class TronTRC20TransferEventJson
    {
        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("transaction_id")]
        public string TransactionHash { get; set; }
    }
}
