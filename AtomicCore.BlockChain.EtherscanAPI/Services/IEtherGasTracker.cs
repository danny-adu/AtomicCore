namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// https://docs.etherscan.io/api-endpoints/gas-tracker
    /// </summary>
    public interface IEtherGasTracker
    {
        /// <summary>
        /// 获取网络手续费（三档）
        /// </summary>
        /// <returns></returns>
        EtherscanSingleResult<EthGasOracleJsonResult> GetGasOracle();
    }
}
