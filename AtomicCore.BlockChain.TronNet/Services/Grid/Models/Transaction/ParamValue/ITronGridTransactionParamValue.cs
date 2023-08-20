namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transaction ParamValue Interface
    /// </summary>
    public interface ITronGridTransactionParamValue
    {
        /// <summary>
        /// ParamValue Parse To Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Parse<T>()
           where T : ITronGridTransactionParamValue, new();

        /// <summary>
        /// include contract adddress
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="isBase58Checked"></param>
        /// <returns></returns>
        bool IncludContractAddress(string contractAddress, bool isBase58Checked = true);
    }
}
