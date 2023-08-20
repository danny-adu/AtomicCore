using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan Result Base
    /// </summary>
    public abstract class EtherscanBaseResult
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EtherscanBaseResult()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        public EtherscanBaseResult(EtherscanJsonStatus status, string msg)
        {
            this.Status = status;
            this.Message = msg;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        public EtherscanBaseResult(EtherscanBaseResult result)
        {
            this.Status = result.Status;
            this.Message = result.Message;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// code
        /// </summary>
        [JsonProperty("status")]
        public EtherscanJsonStatus Status { get; set; }

        /// <summary>
        /// message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// rest url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        #endregion
    }
}
