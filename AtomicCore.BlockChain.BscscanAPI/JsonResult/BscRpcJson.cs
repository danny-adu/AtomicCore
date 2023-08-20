using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc rpc json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class BscRpcJson<T>
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// jsonrpc
        /// </summary>
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
