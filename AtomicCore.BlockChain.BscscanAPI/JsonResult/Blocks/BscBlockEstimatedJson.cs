using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc block estimated json
    /// </summary>
    public class BscBlockEstimatedJson
    {
        /// <summary>
        /// CurrentBlock
        /// </summary>
        [JsonProperty("CurrentBlock")]
        public long CurrentBlock { get; set; }

        /// <summary>
        /// CountdownBlock
        /// </summary>
        [JsonProperty("CountdownBlock")]
        public long CountdownBlock { get; set; }

        /// <summary>
        /// RemainingBlock
        /// </summary>
        [JsonProperty("RemainingBlock")]
        public long RemainingBlock { get; set; }

        /// <summary>
        /// EstimateTimeInSec
        /// </summary>
        [JsonProperty("EstimateTimeInSec")]
        public float EstimateTimeInSec { get; set; }
    }
}
