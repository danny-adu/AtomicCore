using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// Http协议(提供Get Post Download等方法函数)
    /// </summary>
    public static class HttpProtocol
    {
        #region Variables

        /// <summary>
        /// ContentType -> application/json
        /// </summary>
        public const string APPLICATIONJSON = "application/json";

        /// <summary>
        /// ContentType -> application/x-www-form-urlencoded
        /// </summary>
        public const string XWWWFORMURLENCODED = "application/x-www-form-urlencoded";

        #endregion

        #region Public Methods

        /// <summary>
        /// HTTP POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType">application/json | application/x-www-form-urlencoded | .....</param>
        /// <param name="heads"></param>
        /// <param name="chast"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, string contentType = APPLICATIONJSON, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == chast)
                chast = Encoding.UTF8;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";

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
        public static string HttpGet(string url, string data = null, Dictionary<string, string> heads = null, Encoding chast = null)
        {
            if (null == chast)
                chast = Encoding.UTF8;

            string get_url;
            if (string.IsNullOrEmpty(data))
                get_url = url;
            else
                get_url = string.Format("{0}?{1}", url, UrlEncoder.UrlEncode(data, chast));

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            HttpWebRequest request = WebRequest.Create(get_url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";

            if (null != heads && heads.Count > 0)
                foreach (var kv in heads)
                    request.Headers.Add(kv.Key, kv.Value);

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
        /// 下载图片流至内存缓冲字节数组中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contenType"></param>
        /// <returns></returns>
        public static byte[] DownImage(string url, ref string contenType)
        {
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri get_url))
                return null;

            string suffix = get_url.LocalPath.Substring(get_url.LocalPath.LastIndexOf('.'));
            if (string.IsNullOrEmpty(suffix))
                return null;
            else
                suffix = suffix.ToLower();

            if (!s_imgContentTypeDics.ContainsKey(suffix))
                return null;
            else
                contenType = s_imgContentTypeDics[suffix];

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                SetCertificateValidationCallBack();//HTTPS证书验证

            HttpWebRequest request = WebRequest.Create(get_url) as HttpWebRequest;
            request.ServicePoint.Expect100Continue = false;
            request.Method = "GET";
            request.KeepAlive = true;

            request.ContentType = contenType;

            byte[] bys = null;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (System.DrawingCore.Image img = System.DrawingCore.Image.FromStream(response.GetResponseStream()))
                        img.Save(ms, img.RawFormat);

                    bys = ms.ToArray();
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

            return bys;
        }

        #endregion

        #region SSL Certificate Validatrion

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

        #region Private Methods

        /// <summary>
        /// 图片ContentType
        /// </summary>
        private static readonly Dictionary<string, string> s_imgContentTypeDics = new Dictionary<string, string>()
        {
            {".fax"," image/fax " },
            {".gif","image/gif" },
            {".ico","image/x-icon" },
            {".jfif" ,"image/jpeg"},
            {".jpe","image/jpeg" },
            { ".jpeg","image/jpeg"},
            { ".jpg","image/jpg"},
            { ".net","image/pnetvue"},
            { ".png","image/png"},
            {".rp","image/vnd.rn-realpix" },
            {".tif","image/tiff" },
            { ".tiff","image/tiff"},
            {".wbmp"," image/vnd.wap.wbmp " }
        };

        #endregion
    }
}
