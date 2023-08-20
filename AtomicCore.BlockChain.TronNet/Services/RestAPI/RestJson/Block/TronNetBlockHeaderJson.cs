using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Header Json
    /// </summary>
    public class TronNetBlockHeaderJson
    {
        /// <summary>
        /// raw data
        /// </summary>
        [JsonProperty("raw_data")]
        public TronNetBlockHeaderRawJson RawData { get; set; }

        /// <summary>
        /// witness signature
        /// </summary>
        [JsonProperty("witness_signature")]
        public string WitnessSignature { get; set; }
    }
}
