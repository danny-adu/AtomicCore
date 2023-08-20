//using AtomicCore.IOStorage.Core;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Primitives;
//using Microsoft.Net.Http.Headers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AtomicCore.IOStorage.StoragePort.Controllers
//{
//    /// <summary>
//    /// 上传控制器
//    /// https://www.jb51.net/article/174873.htm
//    /// </summary>
//    public class ApiServiceController : BizControllerBase
//    {
//        #region Variable

//        /// <summary>
//        /// 当前WEB路径(相关配置参数)
//        /// </summary>
//        private readonly IBizPathSrvProvider _pathProvider = null;

//        /// <summary>
//        /// 日志实例
//        /// </summary>
//        private readonly ILogger<ApiServiceController> _logger;

//        #endregion

//        #region Constructors

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="pathProvider"></param>
//        /// <param name="logger"></param>
//        public ApiServiceController(IBizPathSrvProvider pathProvider, ILogger<ApiServiceController> logger)
//        {
//            _pathProvider = pathProvider;
//            _logger = logger;
//        }

//        #endregion

//        #region Action Methods

//        /// <summary>
//        /// 流式文件上传（分片多文件上传）
//        /// 直接读取请求体装载后的Section 对应的stream 直接操作strem即可。无需把整个请求体读入内存
//        /// </summary>
//        /// <param name="bizFolder">业务文件夹</param>
//        /// <param name="indexFolder">数据索引文件夹</param>
//        /// <remarks>
//        /// 从多部分请求收到文件，然后应用直接处理或保存它。流式传输无法显著提高性能。流式传输可降低上传文件时对内存或磁盘空间的需求。
//        /// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
//        /// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/2.x/SampleApp/Controllers/StreamingController.cs
//        /// </remarks>
//        /// <returns></returns>
//        [HttpPost]
//        [DisableFormValueModelBinding]
//        public async Task<IActionResult> UploadingStream(string bizFolder, string indexFolder)
//        {
//            //权限判断
//            if (!this.HasPremission)
//                return Ok(new BizIOBatchUploadJsonResult("forbidden"));

//            //基础判断
//            if (string.IsNullOrEmpty(bizFolder))
//                return Ok(new BizIOBatchUploadJsonResult("业务文件夹不允许为空"));

//            //判断内容类型
//            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
//                return Ok(new BizIOBatchUploadJsonResult("contentType must be 'multipart' type..."));

//            //上传文件集合
//            List<string> relativeList = new List<string>();
//            List<string> urlList = new List<string>();

//            //获取boundary
//            MediaTypeHeaderValue mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(Request.ContentType);
//            StringSegment stringSegment = HeaderUtilities.RemoveQuotes(mediaTypeHeaderValue.Boundary);
//            StringSegment boundary = stringSegment.Value;
//            MultipartReader reader = new MultipartReader(boundary.Value, HttpContext.Request.Body);

//            //开始抽取一个地址
//            MultipartSection section = await reader.ReadNextSectionAsync();

//            //优先读取表单提交字段数据
//            while (null != section)
//            {
//                //获取分片内容的描述部分
//                bool hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);

//                //若存在描述则执行下逻辑
//                if (hasContentDispositionHeader)
//                {
//                    //判断是否包含
//                    if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
//                        return Ok(new BizIOBatchUploadJsonResult("No valid file stream was identified..."));

//                    //读取文件流
//                    byte[] buffer = await FileHelpers.ProcessStreamedFile(section, contentDisposition, ModelState, _pathProvider.PermittedExtensions, _pathProvider.FileSizeLimit);
//                    if (!ModelState.IsValid)
//                        return Ok(new BizIOBatchUploadJsonResult(GetError(ModelState)));

//                    //计算文件HASH值
//                    string fileExt = Path.GetExtension(contentDisposition.FileName.Value).ToLowerInvariant();
//                    string fileName = string.Format("{0}{1}", AtomicCore.MD5Handler.Generate(buffer, false), fileExt);

//                    //计算存储路径 + 上传文件
//                    string savePath = GetSaveIOPath(bizFolder, indexFolder, fileName);
//                    await WriteFileAsync(buffer, savePath);

//                    //将文件存储结果返回至集合
//                    string each_relative_path = this.GetRelativePath(bizFolder, indexFolder, fileName);
//                    relativeList.Add(each_relative_path);

//                    string each_url = string.Format(
//                        "{0}://{1}{2}",
//                        this.Request.IsHttps ? "https" : "http",
//                        this.Request.Host.Value,
//                        each_relative_path
//                    );
//                    urlList.Add(each_url);
//                }

//                // Drain any remaining section body that hasn't been consumed and
//                // read the headers for the next section.
//                section = await reader.ReadNextSectionAsync();
//            }

//            //返回成功数据
//            return Ok(new BizIOBatchUploadJsonResult()
//            {
//                Code = BizIOStateCode.Success,
//                Message = string.Empty,
//                RelativeList = relativeList,
//                UrlList = urlList
//            });
//        }

//        /// <summary>
//        /// 缓存式文件上传（单文件上传）
//        /// 通过模型绑定先把整个文件保存到内存，然后我们通过IFormFile得到stream，优点是效率高，缺点对内存要求大。文件不宜过大
//        /// </summary>
//        /// <param name="bizFolder">业务文件夹</param>
//        /// <param name="indexFolder">数据索引文件夹</param>
//        /// <param name="fileName">文件保存名称，用户可自定义指定，若不传则自动计算MD5值</param>
//        /// <remarks>
//        /// 整个文件读入 IFormFile，它是文件的 C# 表示形式，用于处理或保存文件。文件上传所用的资源（磁盘、内存）取决于并发文件上传的数量和大小。 
//        /// 如果应用尝试缓冲过多上传，站点就会在内存或磁盘空间不足时崩溃。如果文件上传的大小或频率会消耗应用资源，请使用流式传输
//        /// </remarks>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<IActionResult> UploadingFormFile(string bizFolder, string indexFolder, string fileName)
//        {
//            //权限判断
//            if (!this.HasPremission)
//                return Ok(new BizIOBatchUploadJsonResult("forbidden"));

//            //基础判断
//            if (null == this.Request.Form.Files || this.Request.Form.Files.Count <= 0)
//                return Ok(new BizIOSingleUploadJsonResult("未检测到上传数据流"));
//            if (string.IsNullOrEmpty(bizFolder))
//                return Ok(new BizIOSingleUploadJsonResult("业务文件夹不允许为空"));

//            //获取文件数据
//            IFormFile file = this.Request.Form.Files.FirstOrDefault();
//            if (null == file || file.Length <= 0)
//                return Ok(new BizIOSingleUploadJsonResult("未检测到上传数据流"));

//            //长度验证、文件格式
//            if (file.Length >= _pathProvider.FileSizeLimit)
//                return Ok(new BizIOSingleUploadJsonResult(string.Format("最大上传不得超过【{0}】M", _pathProvider.FileSizeLimit / (1024 * 1024))));

//            string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
//            if (null != _pathProvider.PermittedExtensions && !_pathProvider.PermittedExtensions.Select(s => s.Trim()).Any(d => d.Equals(fileExt, StringComparison.OrdinalIgnoreCase)))
//                return Ok(new BizIOSingleUploadJsonResult(string.Format("非法的文件格式 -> 当前格式为【{0}】", fileExt)));

//            //读取文件流并保存数据
//            string relativePath;
//            using (Stream stream = file.OpenReadStream())
//            {
//                // 判断上传是否指定保存文件名称,若为空则计算文件HASH值
//                if (string.IsNullOrEmpty(fileName))
//                    fileName = string.Format("{0}{1}", AtomicCore.MD5Handler.Generate(stream, false), fileExt);

//                //计算存储路径 + 上传文件
//                string savePath = GetSaveIOPath(bizFolder, indexFolder, fileName);
//                _logger.LogInformation($"[UploadingFormFile] -> savePath is {savePath},ready to save file!");

//                //开始异步写入磁盘
//                await WriteFileAsync(stream, savePath);

//                //获取当前文件存储的相对路径
//                relativePath = this.GetRelativePath(bizFolder, indexFolder, fileName);
//            }

//            //返回成功数据
//            return Ok(new BizIOSingleUploadJsonResult()
//            {
//                Code = BizIOStateCode.Success,
//                Message = string.Empty,
//                RelativePath = relativePath,
//                Url = string.Format(
//                    "{0}://{1}{2}",
//                    this.Request.IsHttps ? "https" : "http",
//                    this.Request.Host.Value,
//                    relativePath
//                )
//            });
//        }

//        #endregion

//        #region Private Methods

//        /// <summary>
//        /// 获取相对路径
//        /// </summary>
//        /// <param name="bizFolder"></param>
//        /// <param name="indexFolder"></param>
//        /// <param name="fileName"></param>
//        /// <returns></returns>
//        private string GetRelativePath(string bizFolder, string indexFolder, string fileName)
//        {
//            if (string.IsNullOrEmpty(bizFolder))
//                throw new ArgumentNullException(nameof(bizFolder));

//            StringBuilder strb = new("/");
//            strb.Append(_pathProvider.SaveRootDir);
//            strb.Append('/');
//            strb.Append(bizFolder);
//            strb.Append('/');
//            if (string.IsNullOrEmpty(indexFolder))
//                strb.Append(fileName);
//            else
//                strb.AppendFormat("{0}/{1}", indexFolder, fileName);

//            return strb.ToString().ToLower();
//        }

//        /// <summary>
//        /// 获取存储IO路径
//        /// </summary>
//        /// <param name="bizFolder"></param>
//        /// <param name="indexFolder"></param>
//        /// <param name="fileName"></param>
//        /// <returns></returns>
//        private string GetSaveIOPath(string bizFolder, string indexFolder, string fileName)
//        {
//            //判断业务文件夹是否为空
//            if (string.IsNullOrEmpty(bizFolder))
//                throw new ArgumentNullException(nameof(bizFolder));
//            else
//                bizFolder = bizFolder.ToLower();

//            //判断文件名是否为空
//            if (string.IsNullOrEmpty(fileName))
//                throw new ArgumentNullException(nameof(fileName));
//            else
//                fileName = fileName.ToLower();

//            //判断wwwroot文件夹是否存在,防止根目录不存在
//            string io_wwwroot = this._pathProvider.MapPath(string.Empty).ToLower();
//            if (!Directory.Exists(io_wwwroot))
//                Directory.CreateDirectory(io_wwwroot);

//            //判断wwwroot根目录下的逻辑存储根目录是否存在
//            string io_saveRoot = this._pathProvider.MapPath(_pathProvider.SaveRootDir).ToLower();
//            if (!Directory.Exists(io_saveRoot))
//            {
//                _logger.LogInformation($"[GetSaveIOPath] -> save root path '{io_saveRoot}' has not exists,ready to created!");
//                Directory.CreateDirectory(io_saveRoot);
//            }

//            //判断业务模块文件夹是否存在
//            string io_bizFolder = this._pathProvider.MapPath(Path.Combine(_pathProvider.SaveRootDir, bizFolder)).ToLower();
//            if (!Directory.Exists(io_bizFolder))
//            {
//                _logger.LogInformation($"[GetSaveIOPath] -> io_bizFolder path is '{io_bizFolder}' has not exists......");
//                _logger.LogInformation($"[GetSaveIOPath] -> directory '{bizFolder}' has not exists,ready to created!");
//                Directory.CreateDirectory(io_bizFolder);
//            }

//            //判断数据索引文件夹是否存在
//            string io_indexFolder;
//            if (string.IsNullOrEmpty(indexFolder))
//                io_indexFolder = io_bizFolder;
//            else
//            {
//                indexFolder = indexFolder.ToLower();
//                io_indexFolder = this._pathProvider.MapPath(Path.Combine(_pathProvider.SaveRootDir, bizFolder, indexFolder)).ToLower();
//                if (!Directory.Exists(io_indexFolder))
//                {
//                    _logger.LogInformation($"[GetSaveIOPath] -> io_indexFolder path is '{io_indexFolder}' has not exists......");
//                    _logger.LogInformation($"[GetSaveIOPath] -> directory '{indexFolder}' has not exists,ready to created!");
//                    Directory.CreateDirectory(io_indexFolder);
//                }
//            }

//            return Path.Combine(io_indexFolder, fileName);
//        }

//        /// <summary>
//        /// 写文件导到磁盘
//        /// </summary>
//        /// <param name="stream">流</param>
//        /// <param name="path">文件保存路径</param>
//        /// <returns></returns>
//        private static async Task<int> WriteFileAsync(System.IO.Stream stream, string path)
//        {
//            const int FILE_WRITE_SIZE = 84975;
//            int writeCount = 0;

//            using (FileStream fileStream = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, FILE_WRITE_SIZE, true))
//            {
//                int readCount = 0;
//                byte[] byteArr = new byte[FILE_WRITE_SIZE];

//                while ((readCount = await stream.ReadAsync(byteArr.AsMemory(0, byteArr.Length))) > 0)
//                {
//                    await fileStream.WriteAsync(byteArr.AsMemory(0, readCount));
//                    writeCount += readCount;
//                }
//            }

//            return writeCount;
//        }

//        /// <summary>
//        /// 写文件导入磁盘
//        /// </summary>
//        /// <param name="buffer">字节数组</param>
//        /// <param name="path">文件保存路径</param>
//        /// <returns></returns>
//        private static async Task WriteFileAsync(byte[] buffer, string path)
//        {
//            using FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
//            await fs.WriteAsync(buffer.AsMemory(0, buffer.Length));
//        }

//        /// <summary>
//        /// 获取验证模型中的第一个错误
//        /// </summary>
//        /// <param name="modelStateDic"></param>
//        /// <returns></returns>
//        private static string GetError(ModelStateDictionary modelStateDic)
//        {
//            foreach (var item in modelStateDic.Values)
//            {
//                if (item.Errors.Count > 0)
//                {
//                    ModelError mr = item.Errors[0];
//                    if (null != mr.Exception)
//                        return mr.Exception.Message;
//                    else if (!string.IsNullOrEmpty(mr.ErrorMessage))
//                        return mr.ErrorMessage;
//                    else
//                        return "error";
//                }
//            }

//            return string.Empty;
//        }

//        #endregion
//    }
//}
