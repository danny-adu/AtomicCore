using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// 批量上传图片JSON结果
    /// </summary>
    public sealed class BizIOBatchUploadJsonResult : BizIOBaseJsonResult
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public BizIOBatchUploadJsonResult() : base() { }

        /// <summary>
        /// 构造函数（错误）
        /// </summary>
        /// <param name="errorMsg"></param>
        public BizIOBatchUploadJsonResult(string errorMsg) : base(errorMsg)
        {

        }

        #endregion

        #region Propertys

        /// <summary>
        /// 上传返回相对路径列表
        /// </summary>
        [JsonProperty("paths")]
        public List<string> RelativeList { get; set; }

        /// <summary>
        /// 图片链接列表
        /// </summary>
        public List<string> UrlList { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取绝对路径地址
        /// </summary>
        /// <param name="baseUrl">基础URL,eg -> http://static.a.com</param>
        /// <returns></returns>
        public List<string> GetAbsolutePath(string baseUrl)
        {
            if (null == this.RelativeList || this.RelativeList.Count <= 0)
                return null;

            return this.RelativeList.Select(s => string.Format(
                "{0}{1}",
                baseUrl,
                s.StartsWith("/") ? s : string.Format("/{0}", s)
            )).ToList();
        }

        /// <summary>
        /// 获取绝对路径地址
        /// </summary>
        /// <param name="host">主机头</param>
        /// <param name="isSSL">是否ssl</param>
        /// <returns></returns>
        public List<string> GetAbsolutePath(string host, bool isSSL = false)
        {
            if (null == this.RelativeList || this.RelativeList.Count <= 0)
                return null;

            string tmp_http_scheme = isSSL ? "https" : "http";

            return this.RelativeList.Select(s => string.Format(
                "{0}://{1}{2}",
                tmp_http_scheme,
                host,
                s.StartsWith("/") ? s : string.Format("/{0}", s)
            )).ToList();
        }

        #endregion
    }
}
