using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// tron block sync json result
    /// </summary>
    public class TronChainOverviewJson
    {
        /// <summary>
        /// database object
        /// </summary>
        [JsonProperty("database")]
        public TronDatabaseOverviewJson DatabaseOverview { get; set; }

        /// <summary>
        /// sync object
        /// </summary>
        [JsonProperty("sync")]
        public TronSyncOverviewJson SyncOverview { get; set; }

        /// <summary>
        /// network object
        /// </summary>
        [JsonProperty("network")]
        public TronNetworkOverviewJson NetworkOverview { get; set; }

        /// <summary>
        /// full object
        /// </summary>
        [JsonProperty("full")]
        public TronFullNodeOverviewJson FullOverview { get; set; }

        /// <summary>
        /// solidity object
        /// </summary>
        [JsonProperty("solidity")]
        public TronSolidityNodeOverviewJson SolidityOverview { get; set; }
    }
}
