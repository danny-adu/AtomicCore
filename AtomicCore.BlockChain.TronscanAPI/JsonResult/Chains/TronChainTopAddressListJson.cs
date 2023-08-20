using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Chain Top Account List Json
    /// </summary>
    public class TronChainTopAddressListJson : TronPageListJson
    {
        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronChainTopAddressJson[] Data { get; set; }

        /// <summary>
        /// contract maps
        /// </summary>
        [JsonProperty("contractMap")]
        public IReadOnlyDictionary<string, bool> ContractMap { get; set; }

        /// <summary>
        /// Contract Info
        /// </summary>
        public IReadOnlyDictionary<string, TronContractTagJson> ContractInfo { get; set; }
    }
}
