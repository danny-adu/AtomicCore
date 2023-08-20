using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet MachineInfo Json
    /// </summary>
    public class TronNetMachineInfoJson
    {
        /// <summary>
        /// cpuCount
        /// </summary>
        [JsonProperty("cpuCount")]
        public int CpuCount { get; set; }

        /// <summary>
        /// cpuRate
        /// </summary>
        [JsonProperty("cpuRate")]
        public decimal CpuRate { get; set; }

        /// <summary>
        /// deadLockThreadCount
        /// </summary>
        [JsonProperty("deadLockThreadCount")]
        public int DeadLockThreadCount { get; set; }

        /// <summary>
        /// deadLockThreadInfoList
        /// </summary>
        [JsonProperty("deadLockThreadInfoList")]
        public JObject[] DeadLockThreadInfoList { get; set; }

        /// <summary>
        /// freeMemory
        /// </summary>
        [JsonProperty("freeMemory")]
        public ulong FreeMemory { get; set; }

        /// <summary>
        /// javaVersion
        /// </summary>
        [JsonProperty("javaVersion")]
        public string JavaVersion { get; set; }

        /// <summary>
        /// jvmFreeMemory
        /// </summary>
        [JsonProperty("jvmFreeMemory")]
        public ulong JvmFreeMemory { get; set; }

        /// <summary>
        /// jvmTotalMemory
        /// </summary>
        [JsonProperty("jvmTotalMemory")]
        public ulong JvmTotalMemory { get; set; }

        /// <summary>
        /// memoryDescInfoList
        /// </summary>
        [JsonProperty("memoryDescInfoList")]
        public TronNetMachineMemoryDescInfoJson[] MemoryDescInfoList { get; set; }

        /// <summary>
        /// osName
        /// </summary>
        [JsonProperty("osName")]
        public string OsName { get; set; }

        /// <summary>
        /// processCpuRate
        /// </summary>
        [JsonProperty("processCpuRate")]
        public decimal ProcessCpuRate { get; set; }

        /// <summary>
        /// threadCount
        /// </summary>
        [JsonProperty("threadCount")]
        public int ThreadCount { get; set; }

        /// <summary>
        /// totalMemory
        /// </summary>
        [JsonProperty("totalMemory")]
        public ulong TotalMemory { get; set; }
    }
}
