using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Token Asset Json
    /// </summary>
    public class TronTokenAssetJson : TronTokenBasicJson
    {
        /// <summary>
        /// token price intrx
        /// </summary>
        [JsonProperty("tokenPriceInTrx")]
        public decimal TokenPriceInTrx { get; set; }

        /// <summary>
        /// nr of token holders
        /// </summary>
        [JsonProperty("nrOfTokenHolders")]
        public ulong NrOfTokenHolders { get; set; }

        /// <summary>
        /// transfer count
        /// </summary>
        [JsonProperty("transferCount")]
        public ulong TransferCount { get; set; }
    }
}
