using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan的统一返回单个实体对象结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EtherscanSingleResult<T> : EtherscanBaseResult
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EtherscanSingleResult()
            : base()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public EtherscanSingleResult(EtherscanJsonStatus status, string msg, T data = default)
            : base(status, msg)
        {
            this.Result = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        public EtherscanSingleResult(EtherscanBaseResult result, T data)
            : base(result)
        {
            this.Result = data;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 数据结果
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }

        #endregion
    }
}
