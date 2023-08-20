using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Block Info Json
    /// </summary>
    public class TronBlockInfoJson : TronBlockBasicJson
    {
        /// <summary>
        /// Witness Name
        /// </summary>
        [JsonProperty("witnessName")]
        public string WitnessName { get; set; }

        /// <summary>
        /// version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// netUsage
        /// </summary>
        [JsonProperty("netUsage")]
        public int NetUsage { get; set; }

        /// <summary>
        /// energyUsage
        /// </summary>
        [JsonProperty("energyUsage")]
        public int EnergyUsage { get; set; }

        /// <summary>
        /// blockReward
        /// </summary>
        [JsonProperty("blockReward")]
        public int BlockReward { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }
    }
}
