using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transaction Basic Json
    /// </summary>
    public class TronNetTransactionBaseJson : TronNetValidRestJson
    {
        /// <summary>
        /// ret list result
        /// </summary>
        [JsonProperty("ret")]
        public TronNetReturnJson[] Returns { get; set; }

        /// <summary>
        /// signature
        /// </summary>
        [JsonProperty("signature")]
        public string[] Signature { get; set; }

        /// <summary>
        /// txID
        /// </summary>
        [JsonProperty("txID")]
        public string TxID { get; set; }

        /// <summary>
        /// raw_data
        /// </summary>
        [JsonProperty("raw_data")]
        public TronNetTransactionRawDataJson RawData { get; set; }
    }
}
