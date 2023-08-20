using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AtomicCore.IOStorage.StoragePort.GrpcService
{
    /// <summary>
    /// File Grpc Service
    /// </summary>
    public partial class BizFileGrpcService : FileService.FileServiceBase
    {
        #region Variable

        /// <summary>
        /// 头部信息Token
        /// </summary>
        private const string c_head_token = "token";

        /// <summary>
        /// 当前WEB路径(相关配置参数)
        /// </summary>
        private readonly IBizPathSrvProvider _pathProvider;

        /// <summary>
        /// http client factory
        /// </summary>
        protected readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pathProvider"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public BizFileGrpcService(IBizPathSrvProvider pathProvider, IHttpClientFactory httpClientFactory, ILogger<BizFileGrpcService> logger)
        {
            _pathProvider = pathProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        #endregion

        #region Grpc Methods

        /// <summary>
        /// UploadFile
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UploadFileReply> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            // 判断权限
            var requestContext = context.GetHttpContext();
            if (!HasPremission(requestContext))
                return new UploadFileReply()
                {
                    Result = false,
                    Message = "illegal request, insufficient permission to request"
                };

            // 参数接收
            var requests = new Queue<UploadFileRequest>();
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                requests.Enqueue(request);
            }

            // 参数转化
            var first = requests.Peek();
            if (null == first)
                return new UploadFileReply()
                {
                    Result = false,
                    Message = "request data is empty"
                };

            // 判断业务文件夹是否为空
            if (string.IsNullOrEmpty(first.BizFolder))
                return new UploadFileReply()
                {
                    Result = false,
                    Message = "folder name is not allowed to be empty"
                };

            // 拼接上传图片流
            var received = 0L;
            var fileStream = new MemoryStream();
            while (requests.Count > 0)
            {
                var current = requests.Dequeue();
                var buffer = current.FileBytes.ToByteArray();

                fileStream.Seek(received, SeekOrigin.Begin);
                await fileStream.WriteAsync(buffer);

                received += buffer.Length;
            }
            fileStream.Seek(0, SeekOrigin.Begin);

            // 空验证、长度验证
            if (null == fileStream || fileStream.Length <= 0)
                return new UploadFileReply()
                {
                    Result = false,
                    Message = "upload data stream is empty"
                };

            // 格式验证
            string fileExt = string.Empty;
            if (!string.IsNullOrEmpty(first.FileName))
            {
                // 通过文件名称提取后缀
                fileExt = Path.GetExtension(first.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExt))
                    return new UploadFileReply()
                    {
                        Result = false,
                        Message = $"Illegal file format -> The current format is empty"
                    };
                if (null != _pathProvider.PermittedExtensions && !_pathProvider.PermittedExtensions.Select(s => s.Trim()).Any(d => d.Equals(fileExt, StringComparison.OrdinalIgnoreCase)))
                    return new UploadFileReply()
                    {
                        Result = false,
                        Message = $"Illegal file format -> The current format is [{fileExt}]"
                    };
            }
            else
            {
                fileExt = first.FileExt;
                if (string.IsNullOrEmpty(fileExt))
                    return new UploadFileReply()
                    {
                        Result = false,
                        Message = "the file suffix must be specified when specifying the file name"
                    };
            }

            // 判断上传是否指定保存文件名称,若为空则计算文件HASH值
            if (string.IsNullOrEmpty(first.FileName))
                first.FileName = string.Format("{0}{1}", MD5Handler.Generate(fileStream, false), fileExt);

            //计算存储路径 + 上传文件
            string savePath = GetSaveIOPath(first.BizFolder, first.IndexFolder, first.FileName);
            _logger.LogInformation($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> savePath is {savePath}, ready to save file!");

            //开始异步写入磁盘
            await WriteFileAsync(fileStream, savePath);

            //获取当前文件存储的相对路径
            string relativePath = GetRelativePath(first.BizFolder, first.IndexFolder, first.FileName);

            //释放文件流
            fileStream.Dispose();

            return new UploadFileReply()
            {
                Result = true,
                Message = "success",
                RelativePath = relativePath
            };
        }

        /// <summary>
        /// DownloadFile
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileReply> responseStream, ServerCallContext context)
        {
            // 判断权限
            var requestContext = context.GetHttpContext();
            if (!HasPremission(requestContext))
            {
                await responseStream.WriteAsync(new DownloadFileReply()
                {
                    Result = false,
                    Message = "illegal request, insufficient permission to request"
                });
                return;
            }

            // 基础判断
            if (string.IsNullOrEmpty(request.RelativePath))
            {
                await responseStream.WriteAsync(new DownloadFileReply()
                {
                    Result = false,
                    Message = "file path is not allowed to be empty"
                });
                return;
            }

            // 获取文件路径
            string io_file;
            var path_arrs = request.RelativePath.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (_pathProvider.SaveRootDir.Equals(path_arrs.First(), StringComparison.OrdinalIgnoreCase))
                io_file = string.Join(Path.DirectorySeparatorChar, path_arrs);
            else
                io_file = Path.Combine(_pathProvider.SaveRootDir, string.Join(Path.DirectorySeparatorChar, path_arrs));

            // 路劲转换
            var filePath = _pathProvider.MapPath(io_file);
            if (!File.Exists(filePath))
            {
                await responseStream.WriteAsync(new DownloadFileReply()
                {
                    Result = false,
                    Message = "file has not exists"
                });
                return;
            }

            // 读取文件
            using (var fileStream = File.OpenRead(filePath))
            {
                var received = 0L;
                var totalLength = fileStream.Length;

                var buffer = new byte[1024 * 1024]; // 每次最多发送 1M 的文件内容
                while (received < totalLength)
                {
                    var length = await fileStream.ReadAsync(buffer);
                    received += length;

                    var response = new DownloadFileReply()
                    {
                        Result = true,
                        Message = $"{received}/{fileStream.Length}",
                        FileBytes = ByteString.CopyFrom(buffer),
                        TotalSize = (int)fileStream.Length
                    };

                    await responseStream.WriteAsync(response);
                }
            }
        }

        /// <summary>
        /// SnapshotFile
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<SnapshotFileReply> SnapshotFile(SnapshotFileRequest request, ServerCallContext context)
        {
            // 判断权限
            var requestContext = context.GetHttpContext();
            if (!HasPremission(requestContext))
                return new SnapshotFileReply()
                {
                    Result = false,
                    Message = "illegal request, insufficient permission to request"
                };

            // 判断文件下载地址
            if (string.IsNullOrEmpty(request.RemoteUrl))
                return new SnapshotFileReply()
                {
                    Result = false,
                    Message = "remote url is not allowed to be empty"
                };

            // 判断业务文件夹是否为空
            if (string.IsNullOrEmpty(request.BizFolder))
                return new SnapshotFileReply()
                {
                    Result = false,
                    Message = "folder name is not allowed to be empty"
                };
            if (string.IsNullOrEmpty(request.IndexFolder))
                request.IndexFolder = string.Empty;

            // 分析文件后缀
            string fileExt;
            if (request.RemoteUrl.Contains('.', StringComparison.OrdinalIgnoreCase))
            {
                fileExt = request.RemoteUrl[request.RemoteUrl.LastIndexOf('.')..];

                if (fileExt.Contains('?', StringComparison.OrdinalIgnoreCase))
                {
                    Match match = MyRegex().Match(fileExt);
                    if (null == match || !match.Success)
                        fileExt = string.Empty;

                    fileExt = match.Value;
                }

                if (".".Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                    fileExt = string.Empty;
            }
            else
                fileExt = string.Empty;

            // 远程下载文件流
            byte[] buffer;
            using (var client = _httpClientFactory.CreateClient())
            {
                buffer = await client.GetByteArrayAsync(request.RemoteUrl, context.CancellationToken);
            }

            // 判断是否有用户自定义文件名称
            string fileName;
            if (string.IsNullOrEmpty(request.FileName))
                fileName = $"{MD5Handler.Generate(buffer, false)}{fileExt}";
            else
                fileName = $"{request.FileName}{fileExt}";

            //计算存储路径 + 上传文件
            string savePath = GetSaveIOPath(request.BizFolder, request.IndexFolder, fileName);
            _logger.LogInformation($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> snapshot file is {savePath}, ready to save file!");

            //开始异步写入磁盘
            await WriteFileAsync(buffer, savePath);

            //获取当前文件存储的相对路径
            string relativePath = GetRelativePath(request.BizFolder, request.IndexFolder, fileName);

            return new SnapshotFileReply()
            {
                Result = true,
                Message = "success",
                RelativePath = relativePath
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool HasPremission(HttpContext context)
        {
            if (null == _pathProvider)
            {
                _logger.LogError($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> '{nameof(IBizPathSrvProvider)}' is null, are you register the interface of '{nameof(IBizPathSrvProvider)}' in startup?");
                return false;
            }
            if (string.IsNullOrEmpty(_pathProvider.AppToken))
            {
                _logger.LogError($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> '{nameof(_pathProvider.AppToken)}' is null, are you setting the env or appsetting?");
                return false;
            }

            // 判断头部是否包含token
            bool hasHeadToken = context.Request.Headers.TryGetValue(c_head_token, out StringValues headTK);
            if (!hasHeadToken)
            {
                _logger.LogError($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> illegal request, insufficient permission to request");
                return false;
            }

            // 判断Token是否匹配
            if (!_pathProvider.AppToken.Equals(headTK.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError($"[Grpc | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] --> app token is illegal, current request token is '{headTK}'");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="bizFolder"></param>
        /// <param name="indexFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetRelativePath(string bizFolder, string indexFolder, string fileName)
        {
            if (string.IsNullOrEmpty(bizFolder))
                throw new ArgumentNullException(nameof(bizFolder));

            StringBuilder strb = new("/");
            strb.Append(_pathProvider.SaveRootDir);
            strb.Append('/');
            strb.Append(bizFolder);
            strb.Append('/');
            if (string.IsNullOrEmpty(indexFolder))
                strb.Append(fileName);
            else
                strb.AppendFormat("{0}/{1}", indexFolder, fileName);

            return strb.ToString().ToLower();
        }

        /// <summary>
        /// 获取存储IO路径
        /// </summary>
        /// <param name="bizFolder"></param>
        /// <param name="indexFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetSaveIOPath(string bizFolder, string indexFolder, string fileName)
        {
            //判断业务文件夹是否为空
            if (string.IsNullOrEmpty(bizFolder))
                throw new ArgumentNullException(nameof(bizFolder));
            else
                bizFolder = bizFolder.ToLower();

            //判断文件名是否为空
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            else
                fileName = fileName.ToLower();

            //判断wwwroot文件夹是否存在,防止根目录不存在
            string io_wwwroot = _pathProvider.MapPath(string.Empty).ToLower();
            if (!Directory.Exists(io_wwwroot))
                Directory.CreateDirectory(io_wwwroot);

            //判断wwwroot根目录下的逻辑存储根目录是否存在
            string io_saveRoot = _pathProvider.MapPath(_pathProvider.SaveRootDir).ToLower();
            if (!Directory.Exists(io_saveRoot))
            {
                _logger.LogInformation($"[GetSaveIOPath] -> save root path '{io_saveRoot}' has not exists,ready to created!");
                Directory.CreateDirectory(io_saveRoot);
            }

            //判断业务模块文件夹是否存在
            string io_bizFolder = _pathProvider.MapPath(Path.Combine(_pathProvider.SaveRootDir, bizFolder)).ToLower();
            if (!Directory.Exists(io_bizFolder))
            {
                _logger.LogInformation($"[GetSaveIOPath] -> io_bizFolder path is '{io_bizFolder}' has not exists......");
                _logger.LogInformation($"[GetSaveIOPath] -> directory '{bizFolder}' has not exists,ready to created!");
                Directory.CreateDirectory(io_bizFolder);
            }

            //判断数据索引文件夹是否存在
            string io_indexFolder;
            if (string.IsNullOrEmpty(indexFolder))
                io_indexFolder = io_bizFolder;
            else
            {
                indexFolder = indexFolder.ToLower();
                io_indexFolder = _pathProvider.MapPath(Path.Combine(_pathProvider.SaveRootDir, bizFolder, indexFolder)).ToLower();
                if (!Directory.Exists(io_indexFolder))
                {
                    _logger.LogInformation($"[GetSaveIOPath] -> io_indexFolder path is '{io_indexFolder}' has not exists......");
                    _logger.LogInformation($"[GetSaveIOPath] -> directory '{indexFolder}' has not exists,ready to created!");
                    Directory.CreateDirectory(io_indexFolder);
                }
            }

            return Path.Combine(io_indexFolder, fileName);
        }

        /// <summary>
        /// 写文件导到磁盘
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="path">文件保存路径</param>
        /// <returns></returns>
        private static async Task<int> WriteFileAsync(Stream stream, string path)
        {
            const int FILE_WRITE_SIZE = 84975;
            int writeCount = 0;

            using (FileStream fileStream = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FILE_WRITE_SIZE, true))
            {
                int readCount = 0;
                byte[] byteArr = new byte[FILE_WRITE_SIZE];

                while ((readCount = await stream.ReadAsync(byteArr.AsMemory(0, byteArr.Length))) > 0)
                {
                    await fileStream.WriteAsync(byteArr.AsMemory(0, readCount));
                    writeCount += readCount;
                }
            }

            return writeCount;
        }

        /// <summary>
        /// 写文件导入磁盘
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="path">文件保存路径</param>
        /// <returns></returns>
        private static async Task WriteFileAsync(byte[] buffer, string path)
        {
            using FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            await fs.WriteAsync(buffer.AsMemory(0, buffer.Length));
        }

        /// <summary>
        /// 获取验证模型中的第一个错误
        /// </summary>
        /// <param name="modelStateDic"></param>
        /// <returns></returns>
        private static string GetError(ModelStateDictionary modelStateDic)
        {
            foreach (var item in modelStateDic.Values)
            {
                if (item.Errors.Count > 0)
                {
                    ModelError mr = item.Errors[0];
                    if (null != mr.Exception)
                        return mr.Exception.Message;
                    else if (!string.IsNullOrEmpty(mr.ErrorMessage))
                        return mr.ErrorMessage;
                    else
                        return "error";
                }
            }

            return string.Empty;
        }

        [GeneratedRegex("^[^\\?]+(?=\\?)", RegexOptions.IgnoreCase, "zh-CN")]
        private static partial Regex MyRegex();

        #endregion
    }
}
