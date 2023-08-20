using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Account Band Width Json
    /// </summary>
    public class TronAccountBandwidthJson
    {
        /// <summary>
        /// energyRemaining
        /// </summary>
        [JsonProperty("energyRemaining")]
        public ulong EnergyRemaining { get; set; }

        /// <summary>
        /// totalEnergyLimit
        /// </summary>
        [JsonProperty("totalEnergyLimit")]
        public ulong TotalEnergyLimit { get; set; }

        /// <summary>
        /// totalEnergyWeight
        /// </summary>
        [JsonProperty("totalEnergyWeight")]
        public ulong TotalEnergyWeight { get; set; }

        /// <summary>
        /// netUsed
        /// </summary>
        [JsonProperty("netUsed")]
        public ulong NetUsed { get; set; }

        /// <summary>
        /// storageLimit
        /// </summary>
        [JsonProperty("storageLimit")]
        public ulong StorageLimit { get; set; }

        /// <summary>
        /// storagePercentage
        /// </summary>
        [JsonProperty("storagePercentage")]
        public decimal StoragePercentage { get; set; }

        /// <summary>
        /// assets
        /// </summary>
        [JsonProperty("assets")]
        public JObject Assets { get; set; }

        /// <summary>
        /// netPercentage
        /// </summary>
        [JsonProperty("netPercentage")]
        public decimal NetPercentage { get; set; }

        /// <summary>
        /// storageUsed
        /// </summary>
        [JsonProperty("storageUsed")]
        public ulong StorageUsed { get; set; }

        /// <summary>
        /// storageRemaining
        /// </summary>
        [JsonProperty("storageRemaining")]
        public ulong StorageRemaining { get; set; }

        /// <summary>
        /// freeNetLimit
        /// </summary>
        [JsonProperty("freeNetLimit")]
        public ulong FreeNetLimit { get; set; }

        /// <summary>
        /// energyUsed
        /// </summary>
        [JsonProperty("energyUsed")]
        public ulong EnergyUsed { get; set; }

        /// <summary>
        /// freeNetRemaining
        /// </summary>
        [JsonProperty("freeNetRemaining")]
        public ulong FreeNetRemaining { get; set; }

        /// <summary>
        /// netLimit
        /// </summary>
        [JsonProperty("netLimit")]
        public ulong NetLimit { get; set; }

        /// <summary>
        /// netRemaining
        /// </summary>
        [JsonProperty("netRemaining")]
        public ulong NetRemaining { get; set; }

        /// <summary>
        /// energyLimit
        /// </summary>
        [JsonProperty("energyLimit")]
        public ulong EnergyLimit { get; set; }

        /// <summary>
        /// freeNetUsed
        /// </summary>
        [JsonProperty("freeNetUsed")]
        public ulong FreeNetUsed { get; set; }

        /// <summary>
        /// totalNetWeight
        /// </summary>
        [JsonProperty("totalNetWeight")]
        public ulong TotalNetWeight { get; set; }

        /// <summary>
        /// freeNetPercentage
        /// </summary>
        [JsonProperty("freeNetPercentage")]
        public ulong FreeNetPercentage { get; set; }

        /// <summary>
        /// energyPercentage
        /// </summary>
        [JsonProperty("energyPercentage")]
        public ulong EnergyPercentage { get; set; }

        /// <summary>
        /// totalNetLimit
        /// </summary>
        [JsonProperty("totalNetLimit")]
        public ulong TotalNetLimit { get; set; }
    }
}
