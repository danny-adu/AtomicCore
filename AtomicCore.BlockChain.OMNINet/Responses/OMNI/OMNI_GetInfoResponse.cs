namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// OMNI-GetInfo
    /// </summary>
    public class OMNI_GetInfoResponse
    {
        /// <summary>
        /// 客户端版本为整数
        /// </summary>
        public int omnicoreversion_int { get; set; }

        /// <summary>
        /// 客户端版本
        /// </summary>
        public string omnicoreversion { get; set; }

        /// <summary>
        /// 客户端版本（已弃用）
        /// </summary>
        public string mastercoreversion { get; set; }

        /// <summary>
        /// 比特币核心版
        /// </summary>
        public string bitcoincoreversion { get; set; }

        /// <summary>
        /// 最后区块数
        /// </summary>
        public ulong block { get; set; }

        /// <summary>
        /// 最后处理的块的时间戳
        /// </summary>
        public ulong blocktime { get; set; }

        /// <summary>
        /// 在最后处理的块中找到的Omni事务
        /// </summary>
        public ulong blocktransactions { get; set; }

        /// <summary>
        /// 全部处理的Omni交易
        /// </summary>
        public uint totaltransactions { get; set; }

        /// <summary>
        /// 活动协议警报（如果有）
        /// </summary>
        public object[] alerts { get; set; }
    }
}
