using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// TRC10 Transaction List Json
    /// </summary>
    public class TronTRC10TransactionListJson : TronPageListJson
    {
        /// <summary>
        /// trc10 transfer data list
        /// </summary>
        [JsonProperty("Data")]
        public TronTRC10TransactionJson[] TRC10Transfers { get; set; }
    }
}
