// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// RpcRequestTimeoutException
    /// </summary>
    [Serializable]
    public class RpcRequestTimeoutException : Exception
    {
        /// <summary>
        /// RpcRequestTimeoutException
        /// </summary>
        public RpcRequestTimeoutException()
        {
        }

        /// <summary>
        /// RpcRequestTimeoutException
        /// </summary>
        /// <param name="customMessage"></param>
        public RpcRequestTimeoutException(string customMessage) : base(customMessage)
        {
        }

        /// <summary>
        /// RpcRequestTimeoutException
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public RpcRequestTimeoutException(string customMessage, Exception exception) : base(customMessage, exception)
        {
        }
    }
}