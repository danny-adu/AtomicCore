// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// Note: Do not alter the capitalization of the enum members as they are being cast as-is to the RPC server
    /// </summary>
    public enum NodeAction
    {
        /// <summary>
        /// add
        /// </summary>
        add,

        /// <summary>
        /// remove
        /// </summary>
        remove,

        /// <summary>
        /// onetry
        /// </summary>
        onetry
    }
}