using System;

namespace AtomicCore
{
    /// <summary>
    /// DES对称算法接口
    /// </summary>
    public interface IDesSymmetricAlgorithm : IEncryptAlgorithm, IDecryptAlgorithm
    {
        /// <summary>
        /// 获取算法KEY（会自动从conf文件中读取symmetryKey节点配置,或不存在则会启用默认值）
        /// </summary>
        string AlgorithmKey { get; }
    }
}
