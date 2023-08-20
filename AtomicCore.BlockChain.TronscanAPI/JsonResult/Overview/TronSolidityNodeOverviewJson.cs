using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Solidity Node
    /// </summary>
    public class TronSolidityNodeOverviewJson
    {
        /// <summary>
        /// Block Height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }
    }
}
