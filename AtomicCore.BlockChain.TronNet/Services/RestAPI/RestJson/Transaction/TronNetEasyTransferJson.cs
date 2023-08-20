using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Easy Transfer Json
    /// </summary>
    public class TronNetEasyTransferJson
    {

        /// <summary>
        /// TXID
        /// </summary>
        [JsonProperty("txID")]
        public string TxID { get; set; }
    }
}
