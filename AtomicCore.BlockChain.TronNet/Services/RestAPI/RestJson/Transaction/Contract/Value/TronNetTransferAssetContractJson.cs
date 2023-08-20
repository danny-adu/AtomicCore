using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Transfer Asset Contract Json
    /// </summary>
    public class TronNetTransferAssetContractJson : TronNetTransferContractVauleJson
    {
        /// <summary>
        /// Trc10 Asset Name
        /// </summary>
        [JsonProperty("asset_name")]
        public virtual string Trc10AssetName { get; set; }
    }
}
