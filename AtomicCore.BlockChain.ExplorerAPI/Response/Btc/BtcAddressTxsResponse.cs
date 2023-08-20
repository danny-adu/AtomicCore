using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc address txs response
    /// </summary>
    public class BtcAddressTxsResponse : List<BtcTransactionJson>
    {
        /// <summary>
        /// debug url
        /// </summary>
        [JsonIgnore]
        public string DebugUrl { get; set; }
    }
}
