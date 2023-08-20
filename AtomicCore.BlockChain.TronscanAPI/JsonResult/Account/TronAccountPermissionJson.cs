using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// TronNet Account Permission Json
    /// </summary>
    public class TronAccountPermissionJson
    {
        /// <summary>
        /// permission_name
        /// </summary>
        [JsonProperty("permission_name")]
        public string PermissionName { get; set; }

        /// <summary>
        /// accouthresholdnt_name
        /// </summary>
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        /// <summary>
        /// keys
        /// </summary>
        [JsonProperty("keys")]
        public TronAccountPermissionKeyValueJson[] Keys { get; set; }
    }
}
