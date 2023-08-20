using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Net Resource Json
    /// </summary>
    public class TronNetAccountNetResourceJson : TronNetValidRestJson
    {
        /// <summary>
        /// freeNetLimit
        /// </summary>
        [JsonProperty("freeNetLimit")]
        public ulong FreeNetLimit { get; set; }

        /// <summary>
        /// assetNetUsed
        /// </summary>
        [JsonProperty("assetNetUsed")]
        public TronNetAccountResourceKeyValueJson[] AssetNetUsed { get; set; }

        /// <summary>
        /// assetNetLimit
        /// </summary>
        [JsonProperty("assetNetLimit")]
        public TronNetAccountResourceKeyValueJson[] AssetNetLimit { get; set; }


        /// <summary>
        /// TotalNetLimit
        /// </summary>
        [JsonProperty("TotalNetLimit")]
        public ulong TotalNetLimit { get; set; }

        /// <summary>
        /// TotalNetWeight
        /// </summary>
        [JsonProperty("TotalNetWeight")]
        public ulong TotalNetWeight { get; set; }
    }
}
