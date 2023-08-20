using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Operate Permission Json
    /// </summary>
    public class TronNetAccountOperatePermissionJson : TronNetAccountPermissionJson
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
