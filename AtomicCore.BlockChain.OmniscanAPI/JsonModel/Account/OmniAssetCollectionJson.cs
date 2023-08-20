using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni asset collection json
    /// </summary>
    public class OmniAssetCollectionJson
    {
        /// <summary>
        /// asset balances
        /// </summary>
        [JsonProperty("balance")]
        public List<OmniAssetBalanceJson> AssetBalances { get; set; }
    }
}
