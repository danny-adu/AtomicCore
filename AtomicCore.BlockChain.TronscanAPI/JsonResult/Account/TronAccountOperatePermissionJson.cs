using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// TronNet Operate Permission Json
    /// </summary>
    public class TronAccountOperatePermissionJson : TronAccountPermissionJson
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// operations
        /// </summary>
        [JsonProperty("operations")]
        public string Operations { get; set; }
    }
}
