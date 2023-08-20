using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Details Json
    /// </summary>
    public class TronNetBlockDetailsJson : TronNetBlockJson
    {
        /// <summary>
        /// transactions
        /// </summary>
        [JsonProperty("transactions")]
        public TronNetBlockTransactionJson[] Transactions { get; set; }
    }
}
