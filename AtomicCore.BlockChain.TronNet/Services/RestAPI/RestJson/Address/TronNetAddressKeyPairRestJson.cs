using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Address Private Key Rest Json
    /// </summary>
    public class TronNetAddressKeyPairRestJson : TronNetValidRestJson
    {
        /// <summary>
        /// Tron Address Private Key
        /// </summary>
        [JsonProperty("privateKey")]
        public string PrivateKey { get; set; }

        /// <summary>
        /// Tron Address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Tron Hex Address
        /// </summary>
        [JsonProperty("hexAddress")]
        public string HexAddress { get; set; }
    }
}
