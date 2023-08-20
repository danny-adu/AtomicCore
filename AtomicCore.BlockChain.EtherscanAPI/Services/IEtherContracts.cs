namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// https://docs.etherscan.io/api-endpoints/contracts
    /// </summary>
    public interface IEtherContracts
    {
        /// <summary>
        /// 获取合约ABI编码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        EtherscanSingleResult<string> GetContractAbi(string address);
    }
}
