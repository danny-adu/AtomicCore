using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Permission Info
    /// </summary>
    public class TronGridPermissionInfo
    {
        /// <summary>
        /// permission name
        /// </summary>
        [JsonProperty("permission_name")]
        public string PermissionName { get; set; }

        /// <summary>
        /// threshold
        /// </summary>
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        /// <summary>
        /// keys
        /// </summary>
        [JsonProperty("keys")]
        public List<TronGridPermissionKeyInfo> Keys { get; set; }
    }
}
