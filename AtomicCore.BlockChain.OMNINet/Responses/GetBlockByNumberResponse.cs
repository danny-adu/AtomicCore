using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetBlockByNumberResponse
    {
        /// <summary>
        /// 区块哈希值
        /// </summary>
        public string hash { get; set; }

        /// <summary>
        /// 确认次数
        /// </summary>
        public long confirmations { get; set; }

        /// <summary>
        /// 区块的大小
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 区块索引
        /// </summary>
        public long height { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 默克尔树哈希根
        /// </summary>
        public string merkleroot { get; set; }

        /// <summary>
        /// 区块货币价值
        /// </summary>
        public double mint { get; set; }

        /// <summary>
        /// 从1970-01-01的时间戳 单位秒
        /// </summary>
        public ulong time { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// bits
        /// </summary>
        public string bits { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        public double difficulty { get; set; }

        /// <summary>
        /// 前一个区块哈希
        /// </summary>
        public string previousblockhash { get; set; }

        /// <summary>
        /// 后一个区块还行
        /// </summary>
        public string nextblockhash { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string flags { get; set; }

        /// <summary>
        /// proofhash
        /// </summary>
        public string proofhash { get; set; }

        /// <summary>
        /// entropybit
        /// </summary>
        public int entropybit { get; set; }

        /// <summary>
        /// modifier
        /// </summary>
        public string modifier { get; set; }

        /// <summary>
        /// modifierchecksum
        /// </summary>
        public string modifierchecksum { get; set; }

        /// <summary>
        /// tx
        /// </summary>
        public string[] tx { get; set; }

        /// <summary>
        /// signature
        /// </summary>
        public string signature { get; set; }
    }
}
