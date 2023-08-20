using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TypeDecoder Abstract Class
    /// </summary>
    public abstract class TypeDecoder : ITypeDecoder
    {
        #region Abstract Methods

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract bool IsSupportedType(Type type);

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract object Decode(byte[] encoded, Type type);

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public abstract Type GetDefaultDecodingType();

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public T Decode<T>(byte[] encoded)
        {
            return (T) Decode(encoded, typeof(T));
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Decode(string encoded, Type type)
        {
            if (!encoded.StartsWith("0x"))
                encoded = "0x" + encoded;

            return Decode(encoded.HexToByteArray(), type);
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public T Decode<T>(string encoded)
        {
            return (T) Decode(encoded, typeof(T));
        }

        #endregion
    }
}