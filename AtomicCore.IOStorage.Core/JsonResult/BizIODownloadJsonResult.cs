using System.IO;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IO下载Json Result
    /// </summary>
    public sealed class BizIODownloadJsonResult : BizIOBaseJsonResult
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public BizIODownloadJsonResult() : base() { }

        /// <summary>
        /// 构造函数（错误）
        /// </summary>
        /// <param name="errorMsg"></param>
        public BizIODownloadJsonResult(string errorMsg) : base(errorMsg)
        {

        }

        #endregion

        #region Propertys

        /// <summary>
        /// File Stream
        /// </summary>
        public Stream FileStream { get; set; }

        #endregion
    }
}
