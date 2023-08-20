using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// btc transaction position json
    /// </summary>
    public class BtcTransactionPositionJson
    {
        /// <summary>
        /// height
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// positions
        /// </summary>
        [JsonProperty("positions")]
        public int Positions { get; set; }
    }
}
