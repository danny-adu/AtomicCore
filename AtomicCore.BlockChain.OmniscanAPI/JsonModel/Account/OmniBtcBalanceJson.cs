using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni btc balance json
    /// </summary>
    public class OmniBtcBalanceJson
    {
        /// <summary>
        /// btc final balance
        /// </summary>
        [JsonProperty("final_balance")]
        public ulong FinalBalance { get; set; }

        /// <summary>
        /// tx count
        /// </summary>
        [JsonProperty("n_tx")]
        public int NTx { get; set; }

        /// <summary>
        /// total received
        /// </summary>
        [JsonProperty("total_received")]
        public ulong TotalReceived { get; set; }
    }
}
