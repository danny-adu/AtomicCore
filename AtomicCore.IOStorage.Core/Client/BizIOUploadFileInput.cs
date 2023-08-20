using System.IO;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// 单文件上传输入参数
    /// </summary>
    public sealed class BizIOUploadFileInput : BizIORequestInputBase
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
        /// 上传文件名称（上传文件的时候的名称,eg:test.jpg）
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件流对象
        /// </summary>
        public Stream FileStream { get; set; }
    }
}
