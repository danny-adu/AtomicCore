using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ITypeDecoder Interface
    /// </summary>
    public interface ITypeDecoder
    {
        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Decode(byte[] encoded, Type type);

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoded"></param>
        /// <returns></returns>
        T Decode<T>(byte[] encoded);

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Decode(string hexString, Type type);

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hexString"></param>
        /// <returns></returns>
        T Decode<T>(string hexString);

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        Type GetDefaultDecodingType();

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsSupportedType(Type type);
    }
}