using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Block Details Json
    /// </summary>
    public class TronBlockDetailsJson : TronBlockInfoJson
    {
        /// <summary>
        /// transferCount
        /// </summary>
        [JsonProperty("transferCount")]
        public int TransferCount { get; set; }

        /// <summary>
        /// srConfirmList
        /// </summary>
        [JsonProperty("srConfirmList")]
        public TronSRConfirmJson[] SRConfirmList { get; set; }
    }
}
