using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid KV Info
    /// </summary>
    public sealed class TronGridKVInfo
    {
        /// <summary>
        /// key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public System.Numerics.BigInteger Value { get; set; }
    }
}
