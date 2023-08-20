namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ICustomRawDecoder Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICustomRawDecoder<T>
    {
        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        T Decode(byte[] output);
    }
}