using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Resopurce KeyValue Json
    /// </summary>
    public class TronNetAccountResourceKeyValueJson
    {
        /// <summary>
        /// Asset ID
        /// </summary>
        [JsonProperty("key")]
        public string AssetID { get; set; }

        /// <summary>
        /// Used Value
        /// </summary>
        [JsonProperty("value")]
        public ulong UsedValue { get; set; }
    }
}
