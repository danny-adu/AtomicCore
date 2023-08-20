namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 获取指定OMNI交易返回
    /// </summary>
    public class OMNI_GetTransactionResponse
    {
        /// <summary>
        /// 交易TXID
        /// </summary>
        public string txid { get; set; }

        /// <summary>
        /// 手续费（BTC）
        /// </summary>
        public decimal fee { get; set; }

        /// <summary>
        /// 发送地址
        /// </summary>
        public string sendingaddress { get; set; }

        /// <summary>
        /// 接收地址
        /// </summary>
        public string referenceaddress { get; set; }

        /// <summary>
        /// 是否挖矿
        /// </summary>
        public bool ismine { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; set; }

        /// <summary>
        /// 类型代码
        /// </summary>
        public int type_int { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 请求协议（31-USDT）
        /// </summary>
        public ulong propertyid { get; set; }

        /// <summary>
        /// 是否可拆分
        /// </summary>
        public bool divisible { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool valid { get; set; }

        /// <summary>
        /// 区块哈希
        /// </summary>
        public string blockhash { get; set; }

        /// <summary>
        /// 区块时间戳
        /// </summary>
        public ulong blocktime { get; set; }

        /// <summary>
        /// 所在区块中的索引位置
        /// </summary>
        public long positioninblock { get; set; }

        /// <summary>
        /// 区块高度
        /// </summary>
        public long block { get; set; }

        /// <summary>
        /// 已确认数
        /// </summary>
        public long confirmations { get; set; }
    }
}
