using System;
using System.IO;

namespace AtomicCore
{
    /// <summary>
    /// 缓存区拓展类
    /// </summary>
    /// <remarks>seven_880107</remarks>
    public static partial class BufferExtensions
    {
        /// <summary>
        /// 将byte数组转换成内存流
        /// </summary>
        /// <param name="buffer">缓存数组</param>
        /// <param name="offSet">设置内存流起始偏移量,默认为0</param>
        /// <returns></returns>
        public static Stream ToStream(this byte[] buffer, int offSet = 0)
        {
            if (null == buffer || 0 >= buffer.Length)
                throw new ArgumentException("buffer");
            MemoryStream ms = new MemoryStream(buffer);
            ms.Seek(offSet, SeekOrigin.Begin);

            return ms;
        }
    }
}
