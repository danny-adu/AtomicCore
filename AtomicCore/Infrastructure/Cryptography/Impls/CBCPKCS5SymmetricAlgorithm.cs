using System;
using System.Security.Cryptography;
using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// AES - CBC/PKCS5Padding 对称加密/解密算法
    /// </summary>
    public class CBCPKCS5SymmetricAlgorithm : IDesSymmetricAlgorithm
    {
        #region Variable

        /// <summary>
        /// default key
        /// </summary>
        private const string def_consultKey = "gotogether";

        /// <summary>
        /// iterations
        /// </summary>
        private const int iterations = 1000;

        /// <summary>
        /// key size
        /// </summary>
        private const int keySize = 256;

        /// <summary>
        /// block size
        /// </summary>
        private const int blockSize = 128;

        #endregion

        #region IDesSymmetricAlgorithm

        private string _algorithmKey = null;
        private byte[] _saltBytes = null;

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
        /// Salt Bytes
        /// </summary>
        public byte[] SaltBytes
        {
            get
            {
                if (null == _saltBytes)
                    _saltBytes = Encoding.UTF8.GetBytes(MD5Handler.Generate(this.AlgorithmKey, false, Encoding.UTF8));

                return _saltBytes;
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

            #region Salt Calc Key & VI

            Rfc2898DeriveBytes rfc2898;
            if (null == argumentParam || argumentParam.Length <= 0)
                rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(this.AlgorithmKey), SaltBytes, iterations);
            else
            {
                if (argumentParam[0] is string algorithmKey)
                {
                    var tmp_salt = Encoding.UTF8.GetBytes(MD5Handler.Generate(algorithmKey, false, Encoding.UTF8));
                    rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(algorithmKey), tmp_salt, iterations);
                }
                else
                    rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(this.AlgorithmKey), SaltBytes, iterations);
            }

            Span<byte> keyVectorData = rfc2898.GetBytes(keySize / 8 + blockSize / 8);
            var key = keyVectorData.Slice(0, keySize / 8).ToArray();
            var iv = keyVectorData.Slice(keySize / 8).ToArray();

            #endregion

            #region 执行AES加密 

            string result = string.Empty;
            byte[] plainText = Encoding.UTF8.GetBytes(origialText);

            using (var rijndaelCipher = new RijndaelManaged())
            {
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = keySize;
                rijndaelCipher.BlockSize = blockSize;

                rijndaelCipher.Key = key;
                rijndaelCipher.IV = iv;

                byte[] cipherBytes;
                using (ICryptoTransform transform = rijndaelCipher.CreateEncryptor())
                    cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

                result = Convert.ToBase64String(cipherBytes);
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

            #region Salt Calc Key & VI

            Rfc2898DeriveBytes rfc2898;
            if (null == argumentParam || argumentParam.Length <= 0)
                rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(this.AlgorithmKey), SaltBytes, iterations);
            else
            {
                if (argumentParam[0] is string algorithmKey)
                {
                    var tmp_salt = Encoding.UTF8.GetBytes(MD5Handler.Generate(algorithmKey, false, Encoding.UTF8));
                    rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(algorithmKey), tmp_salt, iterations);
                }
                else
                    rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(this.AlgorithmKey), SaltBytes, iterations);
            }

            Span<byte> keyVectorData = rfc2898.GetBytes(keySize / 8 + blockSize / 8);
            var key = keyVectorData.Slice(0, keySize / 8).ToArray();
            var iv = keyVectorData.Slice(keySize / 8).ToArray();

            #endregion

            #region 执行AES解密

            string result = string.Empty;
            byte[] cipher_bytes = Convert.FromBase64String(ciphertext);

            using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
            {
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = keySize;
                rijndaelCipher.BlockSize = blockSize;

                rijndaelCipher.Key = key;
                rijndaelCipher.IV = iv;

                byte[] plainText;
                using (ICryptoTransform transform = rijndaelCipher.CreateDecryptor())
                    plainText = transform.TransformFinalBlock(cipher_bytes, 0, cipher_bytes.Length);

                result = Encoding.UTF8.GetString(plainText);
            }

            #endregion

            return result;
        }

        #endregion
    }
}
