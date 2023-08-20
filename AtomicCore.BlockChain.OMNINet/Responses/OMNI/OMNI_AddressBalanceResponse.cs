namespace AtomicCore.BlockChain.OMNINet
{
    public class OMNI_AddressBalanceResponse
    {
        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }

        /// <summary>
        /// 保留
        /// </summary>
        public decimal reserved { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public decimal frozen { get; set; }
    }
}
