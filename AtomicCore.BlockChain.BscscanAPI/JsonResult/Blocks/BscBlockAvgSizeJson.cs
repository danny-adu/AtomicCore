using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc daily average block size 
    /// </summary>
    public class BscBlockAvgSizeJson
    {
        /// <summary>
        /// UTCDate
        /// </summary>
        [JsonProperty("UTCDate")]
        public string UTCDate { get; set; }

        /// <summary>
        /// unixTimeStamp
        /// </summary>
        [JsonProperty("unixTimeStamp")]
        public long UnixTimeStamp { get; set; }

        /// <summary>
        /// block size bytes
        /// </summary>
        [JsonProperty("blockSize_bytes")]
        public long BlockSizeBytes { get; set; }
    }
}
