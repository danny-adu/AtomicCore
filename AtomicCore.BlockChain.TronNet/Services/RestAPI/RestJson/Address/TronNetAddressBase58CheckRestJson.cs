using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Address Base58Check Rest Json
    /// </summary>
    public class TronNetAddressBase58CheckRestJson : TronNetValidRestJson
    {
        /// <summary>
        /// base58checkAddress
        /// </summary>
        [JsonProperty("base58checkAddress")]
        public string Base58checkAddress { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
