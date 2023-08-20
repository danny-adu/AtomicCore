using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Metadata Json
    /// </summary>
    public class TronContractMetadataJson
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// verify_status
        /// </summary>
        [JsonProperty("verify_status")]
        public int VerifyStatus { get; set; }

        /// <summary>
        /// byteCode
        /// </summary>
        [JsonProperty("byteCode")]
        public string ByteCode { get; set; }

        /// <summary>
        /// abi
        /// </summary>
        [JsonProperty("abi")]
        public string ABI { get; set; }
    }
}
