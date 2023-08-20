namespace AtomicCore.BlockChain.TronNet.Tests
{
    /// <summary>
    /// 测试账户模型
    /// </summary>
    public record TronTestAccount
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; init; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string PirvateKey { get; init; }
    }
}
