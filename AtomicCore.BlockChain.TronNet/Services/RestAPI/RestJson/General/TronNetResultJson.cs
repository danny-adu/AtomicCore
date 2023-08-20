using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Result Json
    /// </summary>
    public class TronNetResultJson : TronNetValidRestJson
    {
        /// <summary>
        /// code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// ContractRet
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; } = false;

        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string TxID { get; set; }
    }
}
