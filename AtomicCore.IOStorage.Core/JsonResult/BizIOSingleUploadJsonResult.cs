using Newtonsoft.Json;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IO上传Json Result
    /// </summary>
    public sealed class BizIOSingleUploadJsonResult : BizIOBaseJsonResult
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public BizIOSingleUploadJsonResult() : base() { }

        /// <summary>
        /// 构造函数（错误）
        /// </summary>
        /// <param name="errorMsg"></param>
        public BizIOSingleUploadJsonResult(string errorMsg) : base(errorMsg)
        {
            
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 上传返回相对路径
        /// </summary>
        [JsonProperty("path")]
        public string RelativePath { get; set; } = string.Empty;

        /// <summary>
        /// 上传的当前站点URL地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取绝对路径地址
        /// </summary>
        /// <param name="baseUrl">基础URL,eg -> http://static.a.com</param>
        /// <returns></returns>
        public string GetAbsolutePath(string baseUrl)
        {
            if (string.IsNullOrEmpty(this.RelativePath))
                return string.Empty;

            return string.Format(
                "{0}{1}",
                baseUrl,
                this.RelativePath.StartsWith("/") ? this.RelativePath : string.Format("/{0}", this.RelativePath)
            );
        }

        /// <summary>
        /// 获取绝对路径地址
        /// </summary>
        /// <param name="host">主机头</param>
        /// <param name="isSSL">是否ssl</param>
        /// <returns></returns>
        public string GetAbsolutePath(string host, bool isSSL = false)
        {
            if (string.IsNullOrEmpty(this.RelativePath))
                return string.Empty;

            string tmp_http_scheme = isSSL ? "https" : "http";

            return string.Format(
                "{0}://{1}{2}",
                tmp_http_scheme,
                host,
                this.RelativePath.StartsWith("/") ? this.RelativePath : string.Format("/{0}", this.RelativePath)
            );
        }

        #endregion
    }
}
