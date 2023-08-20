using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Node Config Json
    /// </summary>
    public class TronNetNodeConfigJson
    {
        /// <summary>
        /// activeNodeSize
        /// </summary>
        [JsonProperty("activeNodeSize")]
        public int ActiveNodeSize { get; set; }

        /// <summary>
        /// allowAdaptiveEnergy
        /// </summary>
        [JsonProperty("allowAdaptiveEnergy")]
        public int AllowAdaptiveEnergy { get; set; }

        /// <summary>
        /// allowCreationOfContracts
        /// </summary>
        [JsonProperty("allowCreationOfContracts")]
        public int AllowCreationOfContracts { get; set; }

        /// <summary>
        /// backupListenPort
        /// </summary>
        [JsonProperty("backupListenPort")]
        public int BackupListenPort { get; set; }

        /// <summary>
        /// backupMemberSize
        /// </summary>
        [JsonProperty("backupMemberSize")]
        public int BackupMemberSize { get; set; }

        /// <summary>
        /// backupPriority
        /// </summary>
        [JsonProperty("backupPriority")]
        public int BackupPriority { get; set; }

        /// <summary>
        /// codeVersion
        /// </summary>
        [JsonProperty("codeVersion")]
        public string CodeVersion { get; set; }

        /// <summary>
        /// dbVersion
        /// </summary>
        [JsonProperty("dbVersion")]
        public int DbVersion { get; set; }

        /// <summary>
        /// discoverEnable
        /// </summary>
        [JsonProperty("discoverEnable")]
        public bool DiscoverEnable { get; set; }

        /// <summary>
        /// listenPort
        /// </summary>
        [JsonProperty("listenPort")]
        public int ListenPort { get; set; }

        /// <summary>
        /// maxConnectCount
        /// </summary>
        [JsonProperty("maxConnectCount")]
        public int MaxConnectCount { get; set; }

        /// <summary>
        /// maxTimeRatio
        /// </summary>
        [JsonProperty("maxTimeRatio")]
        public decimal MaxTimeRatio { get; set; }

        /// <summary>
        /// minParticipationRate
        /// </summary>
        [JsonProperty("minParticipationRate")]
        public int MinParticipationRate { get; set; }

        /// <summary>
        /// minTimeRatio
        /// </summary>
        [JsonProperty("minTimeRatio")]
        public decimal MinTimeRatio { get; set; }

        /// <summary>
        /// p2pVersion
        /// </summary>
        [JsonProperty("p2pVersion")]
        public string P2pVersion { get; set; }

        /// <summary>
        /// passiveNodeSize
        /// </summary>
        [JsonProperty("passiveNodeSize")]
        public int PassiveNodeSize { get; set; }

        /// <summary>
        /// sameIpMaxConnectCount
        /// </summary>
        [JsonProperty("sameIpMaxConnectCount")]
        public int SameIpMaxConnectCount { get; set; }

        /// <summary>
        /// sendNodeSize
        /// </summary>
        [JsonProperty("sendNodeSize")]
        public int SendNodeSize { get; set; }

        /// <summary>
        /// supportConstant
        /// </summary>
        [JsonProperty("supportConstant")]
        public bool SupportConstant { get; set; }

        /// <summary>
        /// versionNum
        /// </summary>
        [JsonProperty("versionNum")]
        public string VersionNum { get; set; }
    }
}
