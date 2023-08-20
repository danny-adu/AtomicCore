// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 数字货币服务接口定义
    /// </summary>
    public interface ICoinService : IRpcService
    {
        /// <summary>
        /// Parameters
        /// </summary>
        CoinParameters Parameters { get; }
    }
}