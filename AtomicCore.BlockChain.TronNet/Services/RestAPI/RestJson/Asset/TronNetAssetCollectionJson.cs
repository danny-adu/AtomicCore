using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Address Asset Json
    /// </summary>
    public class TronNetAssetCollectionJson : TronNetValidRestJson
    {
        /// <summary>
        /// AssetIssue List
        /// </summary>
        [JsonProperty("assetIssue")]
        public TronNetAssetInfoJson[] AssetIssue { get; set; }
    }
}
