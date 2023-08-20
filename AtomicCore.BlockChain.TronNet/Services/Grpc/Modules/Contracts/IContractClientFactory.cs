namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Contract Client Factory Interface
    /// </summary>
    public interface IContractClientFactory
    {
        /// <summary>
        /// Create Client Instance
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        IContractClient CreateClient(ContractProtocol protocol);
    }
}
