using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Valid Rest Json
    /// </summary>
    public abstract class TronNetValidRestJson
    {
        #region Propertys

        /// <summary>
        /// Error Msg
        /// </summary>
        [JsonProperty("Error"), JsonIgnore]
        public string Error { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The Result is available
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAvailable()
        {
            if (null == Error)
                return true;

            return string.IsNullOrEmpty(Error);
        }

        #endregion
    }
}
