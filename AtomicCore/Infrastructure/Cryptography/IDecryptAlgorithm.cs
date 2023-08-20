using System;

namespace AtomicCore
{
    /// <summary>
    /// 解密算法接口
    /// </summary>
    public interface IDecryptAlgorithm
    {
        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <param name="argumentParam">解密辅助参数</param>
        /// <returns></returns>
        string Decrypt(string ciphertext, params object[] argumentParam);

        /// <summary>
        /// 是否是标准的DES密文格式
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <returns></returns>
        bool IsCiphertext(string ciphertext);
    }
}
