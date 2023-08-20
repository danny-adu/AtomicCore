using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// Http Utils
    /// </summary>
    internal static class BizHttpUtils
    {
        #region Variable

        /// <summary>
        /// 默认编码 UTF-8
        /// </summary>
        public static readonly Encoding ENCODING = Encoding.UTF8;

        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        public const string ContentType_XWWWFormUrlencoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// application/json
        /// </summary>
        public const string ContentType_RawJson = "application/json";

        #endregion

        #region Public Methods

        /// <summary>
        /// Http Post application/json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="chast"></param>
        /// <returns></returns>
        public static string HttpJson<T>(string url, T data, Encoding chast = null)
            where T : new()
        {
            if (null == chast)
                chast = ENCODING;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            //序列化
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            //构造请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Timeout = 30 * 60 * 1000;
            if (!string.IsNullOrEmpty(json))
            {
                byte[] buffer = chast.GetBytes(json.ToString());
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();
                }
            }

            string respText = string.Empty;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), chast))
                    respText = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != response)
                    response.Dispose();
            }

            return respText;
        }

        /// <summary>
        /// HTTP POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="inputParams"></param>
        /// <param name="heads"></param>
        /// <param name="chast"></param>
        /// <returns></returns>
        public static string HttpPost(string url, IDictionary<string, string> inputParams, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == inputParams || inputParams.Count <= 0)
                return HttpPost(url, string.Empty, null);
            else
            {
                string data = string.Join("&", inputParams.Select(s => string.Format("{0}={1}", s.Key, s.Value)));
                return HttpPost(url, data, ContentType_XWWWFormUrlencoded, heads, chast);
            }
        }

        /// <summary>
        /// HTTP POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType">application/json | application/x-www-form-urlencoded | .....</param>
        /// <param name="heads"></param>
        /// <param name="chast"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, string contentType = ContentType_XWWWFormUrlencoded, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == chast)
                chast = Encoding.UTF8;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Timeout = 30 * 60 * 1000;

            if (null != heads && heads.Count > 0)
                foreach (var kv in heads)
                    request.Headers.Add(kv.Key, kv.Value);

            if (!string.IsNullOrEmpty(data))
            {
                byte[] buffer = chast.GetBytes(data.ToString());
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();
                }
            }

            string respText = string.Empty;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), chast))
                {
                    respText = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != response)
                    response.Dispose();
            }

            return respText;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="formDatas"></param>
        /// <param name="heads"></param>
        /// <param name="chast"></param>
        /// <remarks>
        /// https://www.cnblogs.com/amylis_chen/p/9699766.html
        /// </remarks>
        /// <returns></returns>
        public static string PostFile(string url, IDictionary<string, byte[]> files, IDictionary<string, string> formDatas, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == files || files.Count < 0)
                return string.Empty;
            if (null == chast)
                chast = ENCODING;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            //0.计算boundary
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Timeout = 30 * 60 * 1000;

            if (null != heads && heads.Count > 0)
                foreach (var kv in heads)
                    request.Headers.Add(kv.Key, kv.Value);

            using (var stream = request.GetRequestStream())
            {
                //1.1 key/value
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (null != formDatas && formDatas.Count > 0)
                {
                    foreach (var kv in formDatas)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, kv.Key, kv.Value);
                        byte[] formitembytes = chast.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                //1.2 file
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                int each_index = 1;
                foreach (var kv in files)
                {
                    string each_fileName = string.Format("file{0}", each_index++);
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, each_fileName, kv.Key);
                    byte[] headerbytes = chast.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    stream.Write(kv.Value, 0, kv.Value.Length);
                }

                //1.3 form end
                stream.Write(endbytes, 0, endbytes.Length);
                stream.Close();
            }

            //2.WebResponse
            string respText = string.Empty;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), chast))
                    respText = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != response)
                    response.Dispose();
            }

            return respText;
        }

        /// <summary>
        /// Http Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="heads"></param>
        /// <param name="chast"></param>
        /// <returns></returns>
        public static string HttpGet(string url, string data, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == chast)
                chast = Encoding.UTF8;

            string get_url;
            if (string.IsNullOrEmpty(data))
                get_url = url;
            else
                get_url = string.Format("{0}?{1}", url, UrlEnconde(data));

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            HttpWebRequest request = WebRequest.Create(get_url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Timeout = 30 * 60 * 1000;

            if (null != heads && heads.Count > 0)
                foreach (var kv in heads)
                    request.Headers.Add(kv.Key, kv.Value);

            string respText = string.Empty;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), chast))
                {
                    respText = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != response)
                    response.Dispose();
            }

            return respText;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 获取多部分表单数据byte数组
        /// </summary>
        /// <param name="postParameters"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private static byte[] GetMultipartFormData(IDictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(ENCODING.GetBytes("\r\n"), 0, ENCODING.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format(
                        "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(ENCODING.GetBytes(header), 0, ENCODING.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format(
                        "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(ENCODING.GetBytes(postData), 0, ENCODING.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(ENCODING.GetBytes(footer), 0, ENCODING.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        /// <summary>
        /// Url参数编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string UrlEnconde(string data)
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var param in data.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv_arr = param.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                queryBuilder.AppendFormat("{0}={1}&", kv_arr.First(), System.Web.HttpUtility.UrlEncode(kv_arr.Last()));
            }
            if (queryBuilder.Length > 1)
                queryBuilder.Remove(queryBuilder.Length - 1, 1);

            return queryBuilder.ToString();
        }

        /// <summary>
        /// 设置服务器证书验证回调
        /// </summary>
        private static void SetCertificateValidationCallBack()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationResult;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        ///  证书验证回调函数  
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cer"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool CertificateValidationResult(object obj, System.Security.Cryptography.X509Certificates.X509Certificate cer, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    /// 文件参数类
    /// https://blog.csdn.net/winterye12/article/details/104005109
    /// </summary>
    public class FileParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileParameter"/> class.
        /// FileParameter.
        /// </summary>
        /// <param name="file">file.</param>
        /// <param name="filename">filename.</param>
        /// <param name="contenttype">contenttype.</param>
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            this.File = file;
            this.FileName = filename;
            this.ContentType = contenttype;
        }

        /// <summary>
        /// Gets or sets File.
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// Gets or sets FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets ContentType.
        /// </summary>
        public string ContentType { get; set; }
    }
}
