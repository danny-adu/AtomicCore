using Newtonsoft.Json;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IO Base Json Result
    /// </summary>
    public class BizIOBaseJsonResult
    {
        #region Constructors

        /// <summary>
        /// Constructors
        /// </summary>
        public BizIOBaseJsonResult()
        {

        }

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="errorMsg"></param>
        public BizIOBaseJsonResult(string errorMsg)
        {
            this.Code = BizIOStateCode.Failure;
            this.Message = errorMsg;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty("code")]
        public BizIOStateCode Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; } = string.Empty;

        #endregion
    }
}
