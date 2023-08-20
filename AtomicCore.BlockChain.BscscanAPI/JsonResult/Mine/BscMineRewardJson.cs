using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// Bsc Mine Reward Json
    /// </summary>
    public class BscMineRewardJson
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
        /// blockReward
        /// </summary>
        [JsonProperty("blockReward"),JsonConverter(typeof(BscBNBConverter))]
        public decimal BlockReward { get; set; }
    }
}
