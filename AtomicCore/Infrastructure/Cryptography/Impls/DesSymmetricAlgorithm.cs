using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AtomicCore
{
    /// <summary>
    /// DES对称加密/解密算法
    /// </summary>
    public class DesSymmetricAlgorithm : IDesSymmetricAlgorithm
    {
        #region Variable

        /// <summary>
        /// 默认协商KEY
        /// </summary>
        public const string def_consultKey = "gotogether";

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

                    this._algorithmKey = MD5Handler.Generate(confKey, true).Top(8);
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
            if (!string.IsNullOrEmpty(ciphertext) && ciphertext.Length > 0 && ciphertext.Length % 2 == 0)
                return Regex.IsMatch(ciphertext, @"^[0-9A-Z]+$");
            else
                return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="origialText"></param>
        /// <param name="argumentParam"></param>
        /// <returns></returns>
        public string Encrypt(string origialText, params object[] argumentParam)
        {
            StringBuilder ret = new StringBuilder();

            #region 获取解密算法 

            string assignKey;
            object obj_argKey = null != argumentParam && argumentParam.Length > 0 ? argumentParam.First() : null;
            if (null == obj_argKey || string.IsNullOrEmpty(obj_argKey.ToString()))
                assignKey = this.AlgorithmKey;
            else
                assignKey = MD5Handler.Generate(obj_argKey.ToString(), true).Top(8);

            ICryptoTransform transform = null;
            using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
            {
                desProvider.Key = ASCIIEncoding.ASCII.GetBytes(assignKey);
                desProvider.IV = ASCIIEncoding.ASCII.GetBytes(assignKey);
                transform = desProvider.CreateEncryptor();
            }

            #endregion

            #region 执行加密

            byte[] inputByteArray;
            byte[] cryptoBytes;
            inputByteArray = Encoding.Default.GetBytes(origialText);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cryptoBytes = ms.ToArray();
                }
            }
            foreach (byte b in cryptoBytes)
            {
                ret.AppendFormat("{0:X2}", b);
            }

            #endregion

            return ret.ToString();
        }

        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="argumentParam"></param>
        /// <returns></returns>
        public string Decrypt(string ciphertext, params object[] argumentParam)
        {
            string originalText = null;

            ciphertext = ciphertext.ToUpper();
            if (!this.IsCiphertext(ciphertext))
                throw new Exception("不是标准的DES密文格式,无法进行解密");

            #region 验证密文

            int len = ciphertext.Length / 2;
            byte[] cipherBytes = new byte[len];
            int x, i;

            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(ciphertext.Substring(x * 2, 2), 16);
                cipherBytes[x] = (byte)i;
            }

            #endregion

            #region 获取解密算法

            string assignKey;
            object obj_argKey = null != argumentParam && argumentParam.Length > 0 ? argumentParam.First() : null;
            if (null == obj_argKey || string.IsNullOrEmpty(obj_argKey.ToString()))
                assignKey = this.AlgorithmKey;
            else
                assignKey = MD5Handler.Generate(obj_argKey.ToString(), true).Top(8);

            ICryptoTransform transform = null;
            using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
            {
                desProvider.Key = ASCIIEncoding.ASCII.GetBytes(assignKey);
                desProvider.IV = ASCIIEncoding.ASCII.GetBytes(assignKey);
                transform = desProvider.CreateDecryptor();
            }

            #endregion

            #region 执行解密

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    originalText = Encoding.Default.GetString(ms.ToArray());
                }
            }

            #endregion

            return originalText;
        }

        #endregion
    }
}
