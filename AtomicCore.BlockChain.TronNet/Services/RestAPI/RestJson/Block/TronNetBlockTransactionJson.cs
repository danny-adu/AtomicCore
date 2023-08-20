using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Transaction Json
    /// </summary>
    public class TronNetBlockTransactionJson : TronNetTransactionBaseJson
    {
        /// <summary>
        /// raw data hex
        /// </summary>
        [JsonProperty("raw_data_hex")]
        public string RawDataHex { get; set; }
    }
}
