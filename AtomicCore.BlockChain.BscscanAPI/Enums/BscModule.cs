namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc模块枚举
    /// </summary>
    public enum BscModule
    {
        /// <summary>
        /// 账户模块
        /// </summary>
        Account = 1,

        /// <summary>
        /// 合约模块
        /// </summary>
        Contract = 2,

        /// <summary>
        /// 交易模块
        /// </summary>
        Transaction = 3,

        /// <summary>
        /// 区块模块
        /// </summary>
        Block = 4,

        /// <summary>
        /// 日志模块
        /// </summary>
        Log = 5,

        /// <summary>
        /// GETH代理模块
        /// </summary>
        Proxy = 6,

        /// <summary>
        /// 代币模块
        /// </summary>
        Token = 7,

        /// <summary>
        /// 手续费模块
        /// </summary>
        GasTracker = 8,

        /// <summary>
        /// 状态模块
        /// </summary>
        Stats = 9
    }
}
