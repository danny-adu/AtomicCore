using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc single result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BscscanSingleResult<T> : BscscanMsgResult
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BscscanSingleResult()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public BscscanSingleResult(BscscanJsonStatus status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }

        #endregion
    }
}
