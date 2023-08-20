using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// Etherscan列表列表结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EtherscanListResult<T> : EtherscanBaseResult
        where T : class, new()
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EtherscanListResult()
            : base()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public EtherscanListResult(EtherscanJsonStatus status, string msg, List<T> data = default)
            : base(status, msg)
        {
            this.Result = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        public EtherscanListResult(EtherscanBaseResult result, List<T> data)
            : base(result)
        {
            this.Result = data;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 列表结果
        /// </summary>
        [JsonProperty("result")]
        public List<T> Result { get; set; }

        #endregion
    }
}
