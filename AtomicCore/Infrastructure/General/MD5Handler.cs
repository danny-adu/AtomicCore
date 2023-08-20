using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// MD5服务处理类
    /// </summary>
    public static class MD5Handler
    {
        /// <summary>
        /// 需要替换的字符
        /// </summary>
        private const string con_replaceChar = "-";

        /// <summary>
        /// 根据一个字符串生成一个MD5摘要信息
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="isToUpper">是否为大写输出</param>
        /// <param name="encoding">编码要求,默认null为uft-8</param>
        /// <returns></returns>
        public static string Generate(string str, bool isToUpper = true, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = UnicodeEncoding.UTF8;

            byte[] bytes = encoding.GetBytes(str);
            return Generate(bytes, isToUpper);
        }

        /// <summary>
        /// 根据一段byte数组生成对应的信息内容摘要
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="isToUpper">是否为大写输出格式</param>
        /// <returns></returns>
        public static string Generate(byte[] bytes, bool isToUpper = true)
        {
            if (null == bytes || bytes.Length == 0)
                return string.Empty;

            string md5_result = null;
            using (MD5 m = new MD5CryptoServiceProvider())
            {
                byte[] s = m.ComputeHash(bytes);
                md5_result = BitConverter.ToString(s).Replace(con_replaceChar, string.Empty);
            }
            return isToUpper ? md5_result.ToUpper() : md5_result.ToLower();
        }

        /// <summary>
        /// 根据一段流生成对应的信息摘要
        /// </summary>
        /// <param name="stream">文件流（文件流使用过方法内部会将游标重置为0）</param>
        /// <param name="isToUpper">是否为大写输出</param>
        /// <returns></returns>
        public static string Generate(Stream stream, bool isToUpper = true)
        {
            if (null == stream || Stream.Null == stream)
                return string.Empty;

            long offset = stream.Position;//偏移量
            long count = stream.Length - offset;//设置还需要读取的量
            byte[] bytes = new byte[count];//设置内存流缓存区
            BinaryReader br = new BinaryReader(stream);
            br.Read(bytes, 0, (int)count);
            stream.Seek(0, SeekOrigin.Begin);

            return Generate(bytes, isToUpper);
        }
    }
}
