namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ITypeEncoder Interface
    /// </summary>
    public interface ITypeEncoder
    {
        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Encode(object value);

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] EncodePacked(object value);
    }
}