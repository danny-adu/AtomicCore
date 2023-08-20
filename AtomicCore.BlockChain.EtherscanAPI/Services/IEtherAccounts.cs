using System.Numerics;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// https://docs.etherscan.io/api-endpoints/accounts
    /// </summary>
    public interface IEtherAccounts
    {
        /// <summary>
        /// 获取地址余额
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">合约地址,若为空则表示为查询主链行为</param>
        /// <param name="contractDecimals">合约代码小数位</param>
        /// <returns></returns>
        EtherscanSingleResult<decimal> GetBalance(string address, string contractAddress = null, int contractDecimals = 0);

        /// <summary>
        /// 获取地址余额(真实最小小数位)
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contractAddress">合约地址,若为空则表示为查询主链行为</param>
        /// <returns></returns>
        EtherscanSingleResult<BigInteger> GetBalanceRaw(string address, string contractAddress = null);

        /// <summary>
        /// 获取交易列表（根据地址）
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="startBlock">起始区块</param>
        /// <param name="endBlock">结束区块</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页容量</param>
        /// <returns></returns>
        EtherscanListResult<EthNormalTransactionJsonResult> GetNormalTransactions(string address, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000);

        /// <summary>
        /// 获取指定地址的内部交易
        /// </summary>
        /// <param name="address">指定地址的内部交易,一般为合约地址</param>
        /// <param name="startBlock">起始区块</param>
        /// <param name="endBlock">结束区块</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页容量</param>
        /// <returns></returns>
        EtherscanListResult<EthInternalTransactionJsonResult> GetInternalTransactions(string address, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000);

        /// <summary>
        /// 获取指定地址的ERC20交易
        /// </summary>
        /// <param name="address">钱包地址</param>
        /// <param name="contract">合约地址,若不传则查询所有与该地址有关系的合约交易</param>
        /// <param name="startBlock">起始区块</param>
        /// <param name="endBlock">结束区块</param>
        /// <param name="sort">排序规则</param>
        /// <param name="page">当前页码</param>
        /// <param name="limit">每页容量</param>
        /// <returns></returns>
        EtherscanListResult<EthErc20TransactionJsonResult> GetERC20Transactions(string address, string contract = null, ulong? startBlock = null, ulong? endBlock = null, EtherscanSort sort = EtherscanSort.Asc, int? page = 1, int? limit = 1000);
    }
}
