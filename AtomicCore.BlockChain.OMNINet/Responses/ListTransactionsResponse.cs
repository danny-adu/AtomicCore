// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 交易列表返回响应对象结果集
    /// </summary>
    public class ListTransactionsResponse
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ListTransactionsResponse()
        {
            this.WalletConflicts = new List<string>();
        }
        /// <summary>
        /// 交易发起帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 交易发起地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 交易类型(例如:发送、接收、......)
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 输出
        /// </summary>
        public int Vout { get; set; }
        /// <summary>
        /// 该笔交易中由于承担的交易费率
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 已确认数量
        /// </summary>
        public int Confirmations { get; set; }
        /// <summary>
        /// 数据块哈希值
        /// </summary>
        public string BlockHash { get; set; }
        /// <summary>
        /// 数据块索引值
        /// </summary>
        public double BlockIndex { get; set; }
        /// <summary>
        /// 数据块产生时间
        /// </summary>
        public double BlockTime { get; set; }
        /// <summary>
        /// 交易编号（主键 唯一）
        /// </summary>
        public string TxId { get; set; }
        /// <summary>
        /// 钱包冲突
        /// </summary>
        public List<string> WalletConflicts { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// 对方接收时间
        /// </summary>
        public double TimeReceived { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 其他账户
        /// </summary>
        public string OtherAccount { get; set; }
        /// <summary>
        /// 是否仅涉及监控
        /// </summary>
        public bool InvolvesWatchonly { get; set; }
    }
}