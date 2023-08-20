using System;
using System.Security.Cryptography;
using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// Advanced Encryption Standard，AES
    /// </summary>
    public class AesSymmetricAlgorithm : IDesSymmetricAlgorithm
    {
        #region Variable

        /// <summary>
        /// default key
        /// </summary>
        private const string def_consultKey = "gotogether";

        #endregion

        #region IDesSymmetricAlgorithm

        private string _algorithmKey = null;

        /// <summary>
        /// 获取算法KEY（会自动从conf文件中读取symmetryKey节点配置,或不存在则会启用默认值）
        /// </summary>
        public string AlgorithmKey
        {
            get
            {
                if (null == this._algorithmKey)
                {
                    string confKey;
                    if (null == ConfigurationJsonManager.AppSettings)
                        confKey = def_consultKey;
                    else
                    {
                        // 针对新版(NETCORE)
                        confKey = ConfigurationJsonManager.AppSettings["SystemWebConfig:SymmetryKey"];
                        if (string.IsNullOrEmpty(confKey))
                        {
                            // 针对旧版(ASP.NET)
                            confKey = ConfigurationJsonManager.AppSettings["symmetryKey"];
                            if (string.IsNullOrEmpty(confKey))
                                confKey = def_consultKey;
                        }
                    }

                    this._algorithmKey = confKey;
                }

                return this._algorithmKey;
            }
        }

        /// <summary>
        /// 是否是标准的DES密文格式
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public bool IsCiphertext(string ciphertext)
        {
            return Base64Handler.IsBase64Format(ciphertext);
        }

        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="origialText">明文字符串</param>
        /// <param name="argumentParam">0:key,1:iv</param>
        /// <returns></returns>
        public string Encrypt(string origialText, params object[] argumentParam)
        {
            #region 基础判断

            if (string.IsNullOrEmpty(origialText))
                throw new ArgumentNullException(nameof(origialText));

            #endregion

            #region 计算KEY

            string key_str;
            if (null == argumentParam || argumentParam.Length <= 0)
                key_str = this.AlgorithmKey;
            else
                if (argumentParam[0] is string algorithmKey)
                key_str = algorithmKey;
            else
                key_str = this.AlgorithmKey;

            var key = Encoding.UTF8.GetBytes(key_str);

            #endregion

            #region 执行AES加密 

            string result = string.Empty;
            byte[] plainText = Encoding.UTF8.GetBytes(origialText);

            using (var rDel = new RijndaelManaged())
            {
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.Key = key;

                byte[] cipherBytes;
                using (ICryptoTransform transform = rDel.CreateEncryptor())
                    cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                result = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="ciphertext">加密字符串</param>
        /// <param name="argumentParam">0:key,1:iv</param>
        /// <returns></returns>
        public string Decrypt(string ciphertext, params object[] argumentParam)
        {
            #region 基础判断

            if (string.IsNullOrEmpty(ciphertext))
                throw new ArgumentNullException(nameof(ciphertext));
            if (!Base64Handler.IsBase64Format(ciphertext))
                throw new ArgumentException($"'{ciphertext}' is not a valid base64!");

            #endregion

            #region 计算KEY

            string key_str;
            if (null == argumentParam || argumentParam.Length <= 0)
                key_str = this.AlgorithmKey;
            else
                if (argumentParam[0] is string algorithmKey)
                key_str = algorithmKey;
            else
                key_str = this.AlgorithmKey;

            var key = Encoding.UTF8.GetBytes(key_str);

            #endregion

            #region 执行AES解密

            string result = string.Empty;
            byte[] cipher_bytes = Convert.FromBase64String(ciphertext);

            using (var rDel = new RijndaelManaged())
            {
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.Key = key;

                byte[] plainText;
                using (ICryptoTransform transform = rDel.CreateDecryptor())
                    plainText = transform.TransformFinalBlock(cipher_bytes, 0, cipher_bytes.Length);

                result = Encoding.UTF8.GetString(plainText);
            }

            #endregion

            return result;
        }

        #endregion
    }
}
