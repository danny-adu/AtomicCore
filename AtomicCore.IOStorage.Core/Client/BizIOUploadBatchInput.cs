using System.IO;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// 批量上传输入参数类
    /// </summary>
    public sealed class BizIOUploadBatchInput : BizIORequestInputBase
    {
        /// <summary>
        /// 业务文件夹名称
        /// </summary>
        public string BizFolder { get; set; }

        /// <summary>
        /// 子文件夹名称(允许为空)
        /// </summary>
        public string SubFolder { get; set; }

        /// <summary>
        /// 多文件流对象
        /// </summary>
        public Stream MultipartStream { get; set; }
    }
}
