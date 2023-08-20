using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node PeerInfo Json
    /// </summary>
    public class TronNetNodePeerInfoJson : TronNetNodeHostJson
    {
        /// <summary>
        /// active
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// avgLatency
        /// </summary>
        [JsonProperty("avgLatency")]
        public decimal AvgLatency { get; set; }

        /// <summary>
        /// blockInPorcSize
        /// </summary>
        [JsonProperty("blockInPorcSize")]
        public int BlockInPorcSize { get; set; }

        /// <summary>
        /// connectTime
        /// </summary>
        [JsonProperty("connectTime")]
        public ulong ConnectTime { get; set; }

        /// <summary>
        /// disconnectTimes
        /// </summary>
        [JsonProperty("disconnectTimes")]
        public ulong DisconnectTimes { get; set; }

        /// <summary>
        /// headBlockTimeWeBothHave
        /// </summary>
        [JsonProperty("headBlockTimeWeBothHave")]
        public ulong HeadBlockTimeWeBothHave { get; set; }

        /// <summary>
        /// headBlockWeBothHave
        /// </summary>
        [JsonProperty("headBlockWeBothHave")]
        public string HeadBlockWeBothHave { get; set; }

        /// <summary>
        /// inFlow
        /// </summary>
        [JsonProperty("inFlow")]
        public int InFlow { get; set; }

        /// <summary>
        /// lastBlockUpdateTime
        /// </summary>
        [JsonProperty("lastBlockUpdateTime")]
        public ulong LastBlockUpdateTime { get; set; }

        /// <summary>
        /// lastSyncBlock
        /// </summary>
        [JsonProperty("lastSyncBlock")]
        public string LastSyncBlock { get; set; }

        /// <summary>
        /// localDisconnectReason
        /// </summary>
        [JsonProperty("localDisconnectReason")]
        public string LocalDisconnectReason { get; set; }

        /// <summary>
        /// needSyncFromPeer
        /// </summary>
        [JsonProperty("needSyncFromPeer")]
        public bool NeedSyncFromPeer { get; set; }

        /// <summary>
        /// needSyncFromUs
        /// </summary>
        [JsonProperty("needSyncFromUs")]
        public bool NeedSyncFromUs { get; set; }

        /// <summary>
        /// nodeCount
        /// </summary>
        [JsonProperty("nodeCount")]
        public int NodeCount { get; set; }

        /// <summary>
        /// nodeId
        /// </summary>
        [JsonProperty("nodeId")]
        public string NodeId { get; set; }

        /// <summary>
        /// remainNum
        /// </summary>
        [JsonProperty("remainNum")]
        public int RemainNum { get; set; }

        /// <summary>
        /// remoteDisconnectReason
        /// </summary>
        [JsonProperty("remoteDisconnectReason")]
        public string RemoteDisconnectReason { get; set; }

        /// <summary>
        /// score
        /// </summary>
        [JsonProperty("score")]
        public decimal Score { get; set; }

        /// <summary>
        /// syncBlockRequestedSize
        /// </summary>
        [JsonProperty("syncBlockRequestedSize")]
        public int SyncBlockRequestedSize { get; set; }

        /// <summary>
        /// syncFlag
        /// </summary>
        [JsonProperty("syncFlag")]
        public bool SyncFlag { get; set; }

        /// <summary>
        /// syncToFetchSize
        /// </summary>
        [JsonProperty("syncToFetchSize")]
        public int SyncToFetchSize { get; set; }

        /// <summary>
        /// syncToFetchSizePeekNum
        /// </summary>
        [JsonProperty("syncToFetchSizePeekNum")]
        public int SyncToFetchSizePeekNum { get; set; }

        /// <summary>
        /// unFetchSynNum
        /// </summary>
        [JsonProperty("unFetchSynNum")]
        public int UnFetchSynNum { get; set; }
    }
}
