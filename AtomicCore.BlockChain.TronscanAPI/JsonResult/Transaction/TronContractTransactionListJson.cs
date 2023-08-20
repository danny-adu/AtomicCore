using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Contract Transaction List Json
    /// </summary>
    public class TronContractTransactionListJson : TronPageListJson
    {
        /// <summary>
        /// Contract Map
        /// </summary>
        [JsonProperty("contractMap")]
        public IReadOnlyDictionary<string, string> ContractMap { get; set; }

        /// <summary>
        /// Contract Info
        /// </summary>
        [JsonProperty("contractInfo")]
        public IReadOnlyDictionary<string, TronContractTagJson> ContractInfo { get; set; }

        /// <summary>
        /// transaction list data
        /// </summary>
        [JsonProperty("data")]
        public TronContractTransactionJson[] Data { get; set; }
    }
}
