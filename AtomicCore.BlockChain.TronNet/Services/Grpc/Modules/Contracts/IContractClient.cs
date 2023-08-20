using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Contract Client Interface
    /// </summary>
    public interface IContractClient
    {
        /// <summary>
        /// Contract Protocol Enum
        /// </summary>
        ContractProtocol Protocol { get; }

        /// <summary>
        /// Transfers
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="ownerAccount"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="memo"></param>
        /// <param name="feeLimit"></param>
        /// <returns></returns>
        Task<string> TransferAsync(string contractAddress, ITronNetAccount ownerAccount, string toAddress, decimal amount, string memo, long feeLimit);

        /// <summary>
        /// BalanceOf 
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="ownerAccount"></param>
        /// <returns></returns>
        Task<decimal> BalanceOfAsync(string contractAddress, ITronNetAccount ownerAccount);
    }
}
