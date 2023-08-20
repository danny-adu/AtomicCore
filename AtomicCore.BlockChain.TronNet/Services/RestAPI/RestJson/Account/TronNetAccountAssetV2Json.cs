using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Asset V2 Jsons
    /// </summary>
    public class TronNetAccountAssetV2Json
    {
        /// <summary>
        /// Asset ID
        /// </summary>
        [JsonProperty("key")]
        public string AssetID { get; set; }

        /// <summary>
        /// Asset Balance
        /// </summary>
        [JsonProperty("value")]
        public ulong AssetBalance { get; set; }
    }
}
