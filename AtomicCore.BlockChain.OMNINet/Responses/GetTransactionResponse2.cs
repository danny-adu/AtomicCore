using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetTransactionResponse2
    {
        public string txid { get; set; }

        public int version { get; set; }

        public ulong time { get; set; }

        public ulong locktime { get; set; }

        public List<Vin> vin { get; set; }

        public List<Vout> vout { get; set; }

        public double amount { get; set; }

        public long confirmations { get; set; }

        public string blockhash { get; set; }

        public long blockindex { get; set; }

        public long blocktime { get; set; }

        public long timereceived { get; set; }

        public string comment { get; set; }

        public string message { get; set; }

        /// <summary>
        /// 转账的简单详情信息
        /// </summary>
        public GetTransactionDetailsInfo2[] details { get; set; }

        /// <summary>
        /// 非转账业务会有该值（GUESS 挖坑的时候会有）
        /// </summary>
        public int txntime { get; set; }
    }

    public class GetTransactionDetailsInfo2
    {
        public string account { get; set; }

        public string address { get; set; }

        public string category { get; set; }

        public double amount { get; set; }
    }
}
