using System;
using System.IO;

namespace AtomicCore
{
    /// <summary>
    /// 二进制流拓展方法
    /// </summary>
    public static partial class StreamExtensions
    {
        /// <summary>
        /// 将流转换至byte数组
        /// </summary>
        /// <param name="stream">流实例</param>
        /// <returns></returns>
        public static byte[] ToBuffer(this Stream stream)
        {
            if (null == stream || Stream.Null == stream)
                throw new ArgumentNullException("stream is null");

            if (stream.Length <= 0)
                throw new Exception("stream length is zero");
            if (stream.Length > int.MaxValue)
                throw new Exception("stream length is too large");

            long offset = stream.Position;//记录流的起始位置

            Stream Current = null;
            if (stream.CanRead)
            {
                Current = stream;
            }
            else
            {
                Current = new MemoryStream();
                stream.CopyTo(Current);
                Current.Seek(0, SeekOrigin.Begin);
            }

            byte[] buffers = new byte[Current.Length];
            BinaryReader br = new BinaryReader(Current);
            br.Read(buffers, 0, (int)Current.Length);

            if (stream.CanSeek)
                stream.Seek(offset, SeekOrigin.Begin);

            return buffers;
        }
    }
}
