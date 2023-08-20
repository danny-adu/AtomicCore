// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetBlockResponse
    {
        /// <summary>
        /// 当前区块下的账单集合
        /// </summary>
        public List<string> Tx { get; set; } = new List<string>();

        /// <summary>
        /// 当前区块的HASH
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// 区块的确认次数
        /// </summary>
        public long Confirmations { get; set; }

        /// <summary>
        /// 区块的大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 区块的索引（从1开始）
        /// </summary>
        public long Height { get; set; }

        /// <summary>
        /// 区块的版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Guess????挖坑奖励给用户的账单号
        /// </summary>
        public string MerkleRoot { get; set; }

        /// <summary>
        /// 挖坑难度
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChainWork { get; set; }

        /// <summary>
        /// 前一区块的256位HASH值
        /// </summary>
        public string PreviousBlockHash { get; set; }

        /// <summary>
        /// 后一区块的256位HASH值
        /// </summary>
        public string NextBlockHash { get; set; }

        /// <summary>
        /// 压缩格式的当前目标HASH值,当调整挖坑难度时会更新
        /// len:4
        /// </summary>
        public string Bits { get; set; }

        /// <summary>
        /// 从1970-01-01 00:00 UTC开始到现在，以秒为单位的当前时间戳
        /// 每几秒就更新
        /// len:4
        /// </summary>
        public ulong Time { get; set; }

        /// <summary>
        /// 从0开始的32位随机
        /// 产生HASH时A (每次产生HASH随机数要增长)
        /// len:4
        /// </summary>
        public string Nonce { get; set; }
    }
}