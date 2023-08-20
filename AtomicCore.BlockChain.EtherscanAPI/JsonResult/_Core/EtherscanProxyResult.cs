using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan Proxy Result
    /// </summary>
    public class EtherscanProxyResult
    {
        #region Propertys

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// jsonrpc
        /// </summary>
        [JsonProperty("jsonrpc")]
        public string Version { get; set; }

        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        #endregion
    }
}
