using System;

namespace AtomicCore
{
    /// <summary>
    /// 加密算法接口
    /// </summary>
    public interface IEncryptAlgorithm
    {
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="origialText">原文</param>
        /// <param name="argumentParam">加密辅助参数</param>
        /// <returns></returns>
        string Encrypt(string origialText, params object[] argumentParam);
    }
}
