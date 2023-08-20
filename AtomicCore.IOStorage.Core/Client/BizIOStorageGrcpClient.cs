using Google.Protobuf;
using Grpc.Net.Client;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IOStorage Grcp Client
    /// </summary>
    public class BizIOStorageGrcpClient
    {
        private const string c_head_token = "token";
        private readonly string _host;
        private readonly int _port;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly int _bufferSize;

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="host">GRPC HOST</param>
        /// <param name="port">GRPC PORT</param>
        /// <param name="baseUrl"></param>
        /// <param name="apiKey">APIKEY</param>
        /// <param name="bufferSize">文件流缓冲区间大小,默认:1M</param>
        public BizIOStorageGrcpClient(string host, int port, string baseUrl, string apiKey, int bufferSize = 1)
        {
            _host = host;
            _port = port;
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// grpc upload file
        /// </summary>
        /// <param name="bizFolder"></param>
        /// <param name="indexFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="fileExt"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task<BizIOSingleUploadJsonResult> UploadFile(string bizFolder, string indexFolder, string fileName, string fileExt, Stream fileStream)
        {
            //基础判断
            if (string.IsNullOrEmpty(bizFolder))
                return new BizIOSingleUploadJsonResult("请指定业务文件夹");
            if (null == fileStream || fileStream.Length <= 0)
                return new BizIOSingleUploadJsonResult("文件流不允许为空");

            // 重置文件流
            fileStream.Seek(0, SeekOrigin.Begin);

            // 设置GRPC头
            var grpcHeads = new Grpc.Core.Metadata
            {
                { c_head_token, _apiKey }
            };

            // 变量定义
            var sended = 0;
            var eachLength = _bufferSize * 1024 * 1024;                 // 每次最多发送 1M 的文件内容
            var totalLength = fileStream.Length;                        // 文件流长度

            UploadFileRequest request;
            UploadFileReply reply;
            string url = $"http://{_host}:{_port}";
            using (var channel = GrpcChannel.ForAddress(url))
            {
                var client = new FileService.FileServiceClient(channel);
                var uploadResult = client.UploadFile(grpcHeads);

                while (sended < totalLength)
                {
                    int length;
                    byte[] buffer;
                    int diff = (int)(totalLength - sended);
                    if (diff > eachLength)
                    {
                        buffer = new byte[eachLength];
                        length = await fileStream.ReadAsync(buffer, 0, eachLength);
                    }
                    else
                    {
                        buffer = new byte[diff];
                        length = await fileStream.ReadAsync(buffer, 0, diff);
                    }

                    sended += length;

                    request = new UploadFileRequest()
                    {
                        BizFolder = bizFolder,
                        IndexFolder = indexFolder ?? string.Empty,
                        FileName = string.IsNullOrEmpty(fileName) ? string.Empty : fileName,
                        FileExt = string.IsNullOrEmpty(fileExt) ? string.Empty : fileExt.StartsWith(".") ? fileExt : $".{fileExt}",
                        FileBytes = ByteString.CopyFrom(buffer)
                    };

                    await uploadResult.RequestStream.WriteAsync(request);
                }

                await uploadResult.RequestStream.CompleteAsync();

                reply = await uploadResult.ResponseAsync;
            }

            return new BizIOSingleUploadJsonResult()
            {
                Code = reply.Result ? BizIOStateCode.Success : BizIOStateCode.Failure,
                Message = reply.Message,
                RelativePath = reply.RelativePath,
                Url = $"{_baseUrl}{reply.RelativePath}"
            };
        }

        /// <summary>
        /// grpc down file
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public async Task<BizIODownloadJsonResult> DownLoadFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return new BizIODownloadJsonResult()
                {
                    Code = BizIOStateCode.Failure,
                    Message = "relative path is empty"
                };

            // 设置GRPC头
            var grpcHeads = new Grpc.Core.Metadata
            {
                { c_head_token, _apiKey }
            };

            // 定义内存留缓冲数据
            MemoryStream fileStream = new MemoryStream();

            // grpc获取数据
            string url = $"http://{_host}:{_port}";
            using (var channel = GrpcChannel.ForAddress(url))
            {
                var client = new FileService.FileServiceClient(channel);
                var downloadResult = client.DownloadFile(new DownloadFileRequest()
                {
                    RelativePath = relativePath
                },
                grpcHeads);

                // 开始接受数据
                var received = 0L;
                while (await downloadResult.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var current = downloadResult.ResponseStream.Current;
                    var buffer = current.FileBytes.ToByteArray();

                    fileStream.Seek(received, SeekOrigin.Begin);
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                    received += buffer.Length;
                    received = Math.Min(received, current.TotalSize);
                }
            }

            return new BizIODownloadJsonResult()
            {
                Code = BizIOStateCode.Success,
                Message = "success",
                FileStream = fileStream
            };
        }

        /// <summary>
        /// grpc snapshot file
        /// </summary>
        /// <param name="bizFolder"></param>
        /// <param name="indexFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="remoterUrl"></param>
        /// <returns></returns>
        public async Task<BizIOSnapshotJsonResult> SnapshotFile(string bizFolder, string indexFolder, string fileName, string remoterUrl)
        {
            //基础判断
            if (string.IsNullOrEmpty(bizFolder))
                return new BizIOSnapshotJsonResult("请指定业务文件夹");
            if (string.IsNullOrEmpty(remoterUrl))
                return new BizIOSnapshotJsonResult("远程路径不允许为空");
            if (!Uri.IsWellFormedUriString(remoterUrl, UriKind.Absolute))
                return new BizIOSnapshotJsonResult("远程路径格式非法");

            // GRPC
            SnapshotFileReply reply;
            string url = $"http://{_host}:{_port}";
            using (var channel = GrpcChannel.ForAddress(url))
            {
                var client = new FileService.FileServiceClient(channel);
                reply = await client.SnapshotFileAsync(new SnapshotFileRequest()
                {
                    BizFolder = bizFolder,
                    IndexFolder = indexFolder ?? string.Empty,
                    FileName = fileName ?? string.Empty,
                    RemoteUrl = remoterUrl
                },
                new Grpc.Core.Metadata
                {
                    { c_head_token, _apiKey }
                });
            }

            return new BizIOSnapshotJsonResult()
            {
                Code = reply.Result ? BizIOStateCode.Success : BizIOStateCode.Failure,
                Message = reply.Message,
                RelativePath = reply.RelativePath,
                Url = $"{_baseUrl}{reply.RelativePath}"
            };
        }
    }
}
