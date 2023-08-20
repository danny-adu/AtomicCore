using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IO存储客户端调用服务类
    /// </summary>
    public class BizIOStorageClient
    {
        /// <summary>
        /// 头部信息Token
        /// </summary>
        private const string c_head_token = "token";

        /// <summary>
        /// 单文件上传
        /// </summary>
        private const string c_singleFile = "/ApiService/UploadingFormFile";

        /// <summary>
        /// 多文件上传
        /// </summary>
        private const string c_batchFile = "/ApiService/UploadingStream";

        /// <summary>
        /// 服务端基础URL
        /// </summary>
        private readonly string _baseUrl = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">服务端URL地址</param>
        public BizIOStorageClient(string baseUrl)
        {
            this._baseUrl = baseUrl;
        }

        /// <summary>
        /// 单文件上传
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        public BizIOSingleUploadJsonResult UploadFile(BizIOUploadFileInput input)
        {
            //基础判断
            if (null == input)
                return new BizIOSingleUploadJsonResult("input参数为空");
            if (string.IsNullOrEmpty(input.BizFolder))
                return new BizIOSingleUploadJsonResult("请指定业务文件夹");
            if (string.IsNullOrEmpty(input.FileName))
                return new BizIOSingleUploadJsonResult("文件名称不允许为空");
            if (null == input.FileStream || input.FileStream.Length <= 0)
                return new BizIOSingleUploadJsonResult("文件流不允许为空");

            //判断是否有apiKey
            Dictionary<string, string> heads = null;
            if (!string.IsNullOrEmpty(input.APIKey))
            {
                heads = new Dictionary<string, string>
                {
                    { c_head_token,input.APIKey }
                };
            }

            //拼接URL
            StringBuilder urlBuilder = new StringBuilder(string.Format(
                "{0}{1}?bizFolder={2}",
                this._baseUrl,
                c_singleFile,
                input.BizFolder
            ));
            if (!string.IsNullOrEmpty(input.SubFolder))
                urlBuilder.AppendFormat("&indexFolder={0}", input.SubFolder);
            if (!string.IsNullOrEmpty(input.FileName))
                urlBuilder.AppendFormat("&fileName={0}", input.FileName);
            urlBuilder.AppendFormat("&rd={0}", DateTime.Now.Ticks.ToString("x"));

            //Stream -> buffer
            byte[] buffer = input.FileStream.ToBuffer();

            //构建文件集合
            Dictionary<string, byte[]> fileDic = new Dictionary<string, byte[]>
            {
                { input.FileName, buffer }
            };

            //请求服务端
            string respText = BizHttpUtils.PostFile(urlBuilder.ToString(), fileDic, null, heads, null);
            if (string.IsNullOrEmpty(respText))
                return new BizIOSingleUploadJsonResult("请求失败");

            //结果集反序列化
            BizIOSingleUploadJsonResult result;
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<BizIOSingleUploadJsonResult>(respText);
            }
            catch (Exception ex)
            {
                return new BizIOSingleUploadJsonResult(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 多文件上传(尚未完成！！！！！！)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BizIOBatchUploadJsonResult UploadBatch(BizIOUploadBatchInput input)
        {
            //基础判断
            if (null == input)
                return new BizIOBatchUploadJsonResult("input参数为空");
            if (string.IsNullOrEmpty(input.BizFolder))
                return new BizIOBatchUploadJsonResult("请指定业务文件夹");
            if (null == input.MultipartStream || input.MultipartStream.Length <= 0)
                return new BizIOBatchUploadJsonResult("文件流不允许为空");

            //判断是否有apiKey
            Dictionary<string, string> heads = null;
            if (!string.IsNullOrEmpty(input.APIKey))
            {
                heads = new Dictionary<string, string>
                {
                    { c_head_token,input.APIKey }
                };
            }

            //拼接URL
            string url = string.Format(
                "{0}{1}",
                this._baseUrl,
                c_batchFile
            );

            //Stream -> buffer
            byte[] buffer = input.MultipartStream.ToBuffer();

            //请求服务端
            string respText = BizHttpUtils.PostFile(url, null, null, heads, null);
            if (string.IsNullOrEmpty(respText))
                return new BizIOBatchUploadJsonResult("请求失败");

            //结果集反序列化
            BizIOBatchUploadJsonResult result;
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<BizIOBatchUploadJsonResult>(respText);
            }
            catch (Exception ex)
            {
                return new BizIOBatchUploadJsonResult(ex.Message);
            }

            return result;
        }
    }
}
