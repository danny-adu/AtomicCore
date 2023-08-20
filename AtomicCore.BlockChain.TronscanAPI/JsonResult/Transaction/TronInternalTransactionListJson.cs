using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Internal Transaction List Json
    /// </summary>
    public class TronInternalTransactionListJson : TronPageListJson
    {
        /// <summary>
        /// Contract Map
        /// </summary>
        [JsonProperty("contractMap")]
        public IReadOnlyDictionary<string, string> ContractMap { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public TronInternalTransactionJson[] Data { get; set; }

        /// <summary>
        /// Contract Info
        /// </summary>
        [JsonProperty("contractInfo")]
        public IReadOnlyDictionary<string, TronContractTagJson> ContractInfo { get; set; }
    }
}
