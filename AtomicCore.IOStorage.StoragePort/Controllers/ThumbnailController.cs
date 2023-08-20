//using System;
//using System.Collections.Generic;
//using System.DrawingCore;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Web;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;

//namespace AtomicCore.IOStorage.StoragePort.Controllers
//{
//    /// <summary>
//    /// 生成缩略图服务
//    /// </summary>
//    public class ThumbnailController : BizControllerBase
//    {
//        #region Variable

//        /// <summary>
//        /// 全局Key,用于分配IO操作KEY
//        /// </summary>
//        private static readonly object s_globalKey = new object();
//        /// <summary>
//        /// IO操作key
//        /// </summary>
//        private static readonly Dictionary<string, object> s_iokeys = new Dictionary<string, object>();

//        /// <summary>
//        /// 配置信息读取
//        /// </summary>
//        private readonly IOptionsMonitor<BizIOStorageConfig> _appSettings;

//        /// <summary>
//        /// 物理路径获取
//        /// </summary>
//        private readonly IBizPathSrvProvider _pathSrv;

//        #endregion

//        #region Constructors

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="appSettings"></param>
//        /// <param name="pathSrv"></param>
//        public ThumbnailController(IOptionsMonitor<BizIOStorageConfig> appSettings, IBizPathSrvProvider pathSrv)
//        {
//            this._appSettings = appSettings;
//            this._pathSrv = pathSrv;
//        }

//        #endregion

//        #region Action Methods

//        /// <summary>
//        /// 获取缩略图(仅限定宽,不限定高)
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="w"></param>
//        /// <returns></returns>
//        [ResponseCache(Duration = 10, VaryByQueryKeys = new string[] { "url", "w" }, Location = ResponseCacheLocation.Client)]
//        public IActionResult Index(string url = "", int w = 0)
//        {
//            url = HttpUtility.UrlDecode(url);
//            if (!this.IsImageUrl(ref url) || w <= 0)
//                return this.NotFound();
//            else
//                url = HttpUtility.UrlDecode(url);

//            //判断源图是否存在
//            string sourceFileIO = this._pathSrv.MapPath(url);
//            if (!System.IO.File.Exists(sourceFileIO))
//                return this.NotFound();

//            //加载获取源图大小
//            Stream sourceFs = new MemoryStream();
//            string contentType = null;
//            int sourceW, sourceH, h;
//            using (Image imgSource = Image.FromFile(sourceFileIO))
//            {
//                //获取源图 宽 高
//                sourceW = imgSource.Width;
//                sourceH = imgSource.Height;

//                //将图片流拷贝进行内存流中
//                imgSource.Save(sourceFs, imgSource.RawFormat);
//            }

//            //如果指定的宽度比源图大，直接返回源图
//            if (w >= sourceW)
//            {
//                sourceFs.Seek(0, SeekOrigin.Begin);
//                contentType = this.GetImageContentType(sourceFs);
//                sourceFs.Seek(0, SeekOrigin.Begin);

//                return this.File(sourceFs, contentType);
//            }
//            else
//            {
//                //按比例计算等比高度
//                double raw = Convert.ToDouble(w) / Convert.ToDouble(sourceW);
//                h = Convert.ToInt32(Math.Floor(Convert.ToDouble(sourceH) * raw));
//            }

//            //获取源文件名称
//            string basePath = url.Substring(0, url.LastIndexOf('/') + 1);//结尾带/
//            string fileName = url.Substring(url.LastIndexOf('/') + 1);//开头不带/
//            string[] splits = fileName.Split('.');

//            //判断图片的缩略图的IO是否存在
//            string thumbPath = string.Format("{0}{1}_{2}_{3}.{4}", basePath, splits.First(), w, h, splits.Last());
//            string thumbIO = this._pathSrv.MapPath(thumbPath);

//            Stream fs = null;
//            FileInfo thumbFileInfo = new FileInfo(thumbIO);
//            if (thumbFileInfo.Exists && thumbFileInfo.Length > 0)
//            {
//                fs = new FileStream(thumbIO, FileMode.Open, FileAccess.Read);
//                contentType = this.GetImageContentType(fs);
//                fs.Seek(0, SeekOrigin.Begin);
//            }
//            else
//            {
//                //单线程创建缩略图并且保存
//                object ioKey = this.GetIOKey(fileName);
//                lock (ioKey)
//                {
//                    thumbFileInfo = new FileInfo(thumbIO);
//                    if (thumbFileInfo.Exists && thumbFileInfo.Length > 0)
//                    {
//                        fs = new FileStream(thumbIO, FileMode.Open, FileAccess.Read);
//                        contentType = this.GetImageContentType(fs);
//                        fs.Seek(0, SeekOrigin.Begin);
//                    }
//                    else
//                    {
//                        //根据源文件生成缩略图
//                        sourceFs.Seek(0, SeekOrigin.Begin);
//                        fs = AtomicCore.ImageHandler.MakeThumbnail(sourceFs, w, h, ImageThumbStrategy.Cut);

//                        //保存源文件
//                        using (FileStream thumbFs = new FileStream(thumbIO, FileMode.OpenOrCreate, FileAccess.ReadWrite))
//                        {
//                            fs.CopyTo(thumbFs);
//                        }
//                        fs.Seek(0, SeekOrigin.Begin);
//                        contentType = this.GetImageContentType(fs);
//                        fs.Seek(0, SeekOrigin.Begin);
//                    }
//                }
//            }
//            return this.File(fs, contentType);
//        }

//        /// <summary>
//        /// 获取缩略图
//        /// </summary>
//        /// <param name="url">源图地址,相对地址</param>
//        /// <returns></returns>
//        [ResponseCache(Duration = 10, VaryByQueryKeys = new string[] { "url", "w", "h" }, Location = ResponseCacheLocation.Client)]
//        public IActionResult Fixed(string url = "", int w = 0, int h = 0)
//        {
//            url = HttpUtility.UrlDecode(url);
//            if (!this.IsImageUrl(ref url) || w <= 0 || h <= 0)
//                return this.NotFound();
//            else
//                url = HttpUtility.UrlDecode(url);

//            //获取文件名称
//            string basePath = url.Substring(0, url.LastIndexOf('/') + 1);//结尾带/
//            string fileName = url.Substring(url.LastIndexOf('/') + 1);//开头不带/
//            string[] splits = fileName.Split('.');

//            //判断图片的缩略图的IO是否存在
//            string thumbPath = string.Format("{0}{1}_{2}_{3}.{4}", basePath, splits.First(), w, h, splits.Last());
//            string thumbIO = this._pathSrv.MapPath(thumbPath);

//            Stream fs = null;
//            string contentType = null;
//            FileInfo thumbFileInfo = new FileInfo(thumbIO);
//            if (thumbFileInfo.Exists && thumbFileInfo.Length > 0)
//            {
//                fs = new FileStream(thumbIO, FileMode.Open, FileAccess.Read);
//                contentType = this.GetImageContentType(fs);
//                fs.Seek(0, SeekOrigin.Begin);
//            }
//            else
//            {
//                //判断源文件是否存在
//                string sourceFileIO = this._pathSrv.MapPath(url);
//                if (!System.IO.File.Exists(sourceFileIO))
//                    return this.NotFound();

//                //单线程创建缩略图并且保存
//                object ioKey = this.GetIOKey(fileName);
//                lock (ioKey)
//                {
//                    thumbFileInfo = new FileInfo(thumbIO);
//                    if (thumbFileInfo.Exists && thumbFileInfo.Length > 0)
//                    {
//                        fs = new FileStream(thumbIO, FileMode.Open, FileAccess.Read);
//                        contentType = this.GetImageContentType(fs);
//                        fs.Seek(0, SeekOrigin.Begin);
//                    }
//                    else
//                    {
//                        //读取源文件的流
//                        using (Image imgSource = Image.FromFile(sourceFileIO))
//                        {
//                            if (w > imgSource.Width)
//                            {
//                                w = imgSource.Width;
//                            }
//                            if (h > imgSource.Height)
//                            {
//                                h = imgSource.Height;
//                            }
//                            using (MemoryStream ms = new MemoryStream())
//                            {
//                                imgSource.Save(ms, imgSource.RawFormat);
//                                ms.Seek(0, SeekOrigin.Begin);

//                                fs = AtomicCore.ImageHandler.MakeThumbnail(ms, w, h, ImageThumbStrategy.Cut);

//                                //保存源文件
//                                using (FileStream thumbFs = new FileStream(thumbIO, FileMode.OpenOrCreate, FileAccess.ReadWrite))
//                                {
//                                    fs.CopyTo(thumbFs);
//                                }
//                                fs.Seek(0, SeekOrigin.Begin);
//                                contentType = this.GetImageContentType(fs);
//                                fs.Seek(0, SeekOrigin.Begin);
//                            }
//                        }
//                    }
//                }
//            }
//            return this.File(fs, contentType);
//        }

//        #endregion

//        #region Private Methods

//        /// <summary>
//        /// 是否是有效的图片地址
//        /// </summary>
//        /// <param name="imgUrl">格式例如:/Upload/Goods/a4ada96eacbb441fa2ba9161c77ef8f4/maps/265691B7EA152A645438B85EF79F7AE6.jpg</param>
//        /// <returns></returns>
//        private bool IsImageUrl(ref string imgUrl)
//        {
//            if (imgUrl.StartsWith("http"))
//            {
//                if (!Uri.TryCreate(imgUrl, UriKind.RelativeOrAbsolute, out Uri uri))
//                    return false;

//                imgUrl = uri.AbsolutePath;
//            }
//            return Regex.IsMatch(imgUrl, @"^\/upload.+\/[0-9a-zA-Z]+\.(?:(?:jpg)|(?:jpeg)|(?:gif)|(?:png))$", RegexOptions.IgnoreCase);
//        }

//        /// <summary>
//        /// 根据图片流获取该流的图片格式
//        /// </summary>
//        /// <param name="fileStream"></param>
//        /// <returns></returns>
//        private string GetImageContentType(Stream fileStream)
//        {
//            string contentType;
//            string fileExt = AtomicCore.ImageHandler.GetExtension(fileStream, false);//获取后缀
//            if (string.IsNullOrEmpty(fileExt))
//                contentType = "*/*";
//            else
//            {
//                contentType = (fileExt.ToLower()) switch
//                {
//                    "jpg" => "image/jpeg",
//                    "jpeg" => "image/jpeg",
//                    "png" => "image/png",
//                    "gif" => "image/gif",
//                    _ => "image/*",
//                };
//            }

//            return contentType;
//        }

//        /// <summary>
//        /// 获取文件的操作Key
//        /// </summary>
//        /// <param name="fileName">源文件名称</param>
//        /// <returns></returns>
//        private object GetIOKey(string fileName)
//        {
//            if (!s_iokeys.ContainsKey(fileName))
//            {
//                lock (s_globalKey)
//                {
//                    if (!s_iokeys.ContainsKey(fileName))
//                    {
//                        s_iokeys.Add(fileName, new object());
//                    }
//                }
//            }

//            return s_iokeys[fileName];
//        }

//        #endregion
//    }
//}
