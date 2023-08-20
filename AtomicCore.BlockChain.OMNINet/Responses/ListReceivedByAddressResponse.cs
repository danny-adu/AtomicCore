// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 接收账户地址响应类型
    /// </summary>
    public class ListReceivedByAddressResponse
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ListReceivedByAddressResponse()
        {
            this.TxIds = new List<string>();
        }

        /// <summary>
        /// 账户名称(lab)
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 账户地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 确认数量
        /// </summary>
        public int Confirmations { get; set; }
        /// <summary>
        /// 隶属的交易ID
        /// </summary>
        public List<string> TxIds { get; set; }
    }
}