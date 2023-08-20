using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Resource Json
    /// </summary>
    public class TronNetAccountResourceJson : TronNetValidRestJson
    {
        /// <summary>
        /// freeNetLimit
        /// </summary>
        [JsonProperty("freeNetLimit")]
        public ulong FreeNetLimit { get; set; }

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

        /// <summary>
        /// tronPowerLimit
        /// </summary>
        [JsonProperty("tronPowerLimit")]
        public ulong TronPowerLimit { get; set; }

        /// <summary>
        /// EnergyLimit
        /// </summary>
        [JsonProperty("EnergyLimit")]
        public ulong EnergyLimit { get; set; }

        /// <summary>
        /// TotalEnergyLimit
        /// </summary>
        [JsonProperty("TotalEnergyLimit")]
        public ulong TotalEnergyLimit { get; set; }

        /// <summary>
        /// TotalEnergyWeight
        /// </summary>
        [JsonProperty("TotalEnergyWeight")]
        public ulong TotalEnergyWeight { get; set; }

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
    }
}
