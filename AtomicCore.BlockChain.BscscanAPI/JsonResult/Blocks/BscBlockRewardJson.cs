using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc block reward json
    /// </summary>
    public class BscBlockRewardJson
    {
        /// <summary>
        /// blockNumber
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }

        /// <summary>
        /// timeStamp
        /// </summary>
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// blockMiner
        /// </summary>
        [JsonProperty("blockMiner")]
        public string BlockMiner { get; set; }

        /// <summary>
        /// blockReward
        /// </summary>
        [JsonProperty("blockReward")]
        public decimal BlockReward { get; set; }

        /// <summary>
        /// uncles
        /// </summary>
        [JsonProperty("uncles")]
        public JArray Uncles { get; set; }

        /// <summary>
        /// uncleInclusionReward
        /// </summary>
        [JsonProperty("uncleInclusionReward")]
        public decimal UncleInclusionReward { get; set; }
    }
}
