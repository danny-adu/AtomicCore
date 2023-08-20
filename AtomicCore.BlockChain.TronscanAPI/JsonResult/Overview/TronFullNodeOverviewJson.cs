using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Full Node Overview
    /// </summary>
    public class TronFullNodeOverviewJson
    {
        /// <summary>
        /// Block Height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }
    }
}
