using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Transaction Client Interface
    /// </summary>
    public interface ITronNetTransactionClient
    {
        /// <summary>
        /// Create Raw Transaction
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount);

        /// <summary>
        /// Raw Transaction Sign
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        Transaction GetTransactionSign(Transaction transaction, string privateKey);

        /// <summary>
        /// Broadcast Transaction On Chain
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<Return> BroadcastTransactionAsync(Transaction transaction);
    }
}
