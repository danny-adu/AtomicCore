using System;
using System.Runtime.Serialization;

namespace AtomicCore
{
    /// <summary>
    /// 谷歌身份认证异常对象类
    /// </summary>
    internal sealed class GoogleOneTimePasswordException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GoogleOneTimePasswordException()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public GoogleOneTimePasswordException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public GoogleOneTimePasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public GoogleOneTimePasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
