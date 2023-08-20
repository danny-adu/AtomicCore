using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet SignedTransaction Rest Json
    /// </summary>
    public class TronNetSignedTransactionRestJson : TronNetCreateTransactionRestJson
    {
        /// <summary>
        /// Signature
        /// </summary>
        [JsonProperty("signature")]
        public string[] Signature { get; set; }
    }
}
