using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node Overview Json
    /// </summary>
    public class TronNetNodeOverviewJson : TronNetValidRestJson
    {
        /// <summary>
        /// activeConnectCount
        /// </summary>
        [JsonProperty("activeConnectCount")]
        public int ActiveConnectCount { get; set; }

        /// <summary>
        /// beginSyncNum
        /// </summary>
        [JsonProperty("beginSyncNum")]
        public ulong BeginSyncNum { get; set; }

        /// <summary>
        /// block
        /// </summary>
        [JsonProperty("block")]
        public string Block { get; set; }

        /// <summary>
        /// cheatWitnessInfoMap
        /// </summary>
        [JsonProperty("cheatWitnessInfoMap")]
        public JObject CheatWitnessInfoMap { get; set; }

        /// <summary>
        /// configNodeInfo
        /// </summary>
        [JsonProperty("configNodeInfo")]
        public TronNetNodeConfigJson ConfigNodeInfo { get; set; }

        /// <summary>
        /// currentConnectCount
        /// </summary>
        [JsonProperty("currentConnectCount")]
        public int CurrentConnectCount { get; set; }

        /// <summary>
        /// machineInfo
        /// </summary>
        [JsonProperty("machineInfo")]
        public TronNetMachineInfoJson MachineInfo { get; set; }

        /// <summary>
        /// passiveConnectCount
        /// </summary>
        [JsonProperty("passiveConnectCount")]
        public int PassiveConnectCount { get; set; }

        /// <summary>
        /// peerList
        /// </summary>
        [JsonProperty("peerList")]
        public TronNetNodePeerInfoJson[] PeerList { get; set; }

        /// <summary>
        /// solidityBlock
        /// </summary>
        [JsonProperty("solidityBlock")]
        public string SolidityBlock { get; set; }

        /// <summary>
        /// totalFlow
        /// </summary>
        [JsonProperty("totalFlow")]
        public ulong TotalFlow { get; set; }
    }
}
