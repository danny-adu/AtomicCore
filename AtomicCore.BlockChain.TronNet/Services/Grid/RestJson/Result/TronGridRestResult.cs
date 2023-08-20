using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Rest Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class TronGridRestResult<T>
    {
        #region Propertys

        /// <summary>
        /// success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// error
        /// </summary>
        [JsonProperty("error"), JsonIgnore]
        public string Error { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public T[] Data { get; set; }

        /// <summary>
        /// meta
        /// </summary>
        [JsonProperty("meta")]
        public TronGridMetaInfo Meta { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The Result is available
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            if (null == Error)
                return true;

            return string.IsNullOrEmpty(Error);
        }

        #endregion
    }
}
