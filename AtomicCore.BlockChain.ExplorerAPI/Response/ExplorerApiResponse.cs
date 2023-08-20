using Newtonsoft.Json;

namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// Explorer Api Response
    /// </summary>
    public abstract class ExplorerApiResponse
    {
        /// <summary>
        /// debug url
        /// </summary>
        [JsonIgnore]
        public string DebugUrl { get; set; }
    }
}
