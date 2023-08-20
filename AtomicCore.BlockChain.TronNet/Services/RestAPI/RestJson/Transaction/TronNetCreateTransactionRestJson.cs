using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Create Transaction Rest Json
    /// </summary>
    public class TronNetCreateTransactionRestJson : TronNetValidRestJson
    {
        /// <summary>
        /// visible
        /// </summary>
        [JsonProperty("visible")]
        public bool Visible { get; set; }

        /// <summary>
        /// TXID
        /// </summary>
        [JsonProperty("txID")]
        public string TxID { get; set; }

        /// <summary>
        /// Raw Data
        /// </summary>
        [JsonProperty("raw_data")]
        public TronNetCreateTransactionRawDataJson RawData { get; set; }

        /// <summary>
        /// Raw Data Hex
        /// </summary>
        [JsonProperty("raw_data_hex")]
        public string RawDataHex { get; set; }
    }
}
