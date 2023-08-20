using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc api response
    /// </summary>
    public class BscscanMsgResult
    {
        #region Propertys

        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("status")]
        public BscscanJsonStatus Status { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        #endregion
    }
}
