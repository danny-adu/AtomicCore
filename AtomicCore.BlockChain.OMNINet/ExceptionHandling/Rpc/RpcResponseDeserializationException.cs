// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// Rpc Response Deserialization Exception
    /// </summary>
    [Serializable]
    public class RpcResponseDeserializationException : Exception
    {
        /// <summary>
        /// RpcResponseDeserializationException
        /// </summary>
        public RpcResponseDeserializationException()
        {
        }

        /// <summary>
        /// RpcResponseDeserializationException
        /// </summary>
        /// <param name="customMessage"></param>
        public RpcResponseDeserializationException(string customMessage) : base(customMessage)
        {
        }

        /// <summary>
        /// RpcResponseDeserializationException
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public RpcResponseDeserializationException(string customMessage, Exception exception) : base(customMessage, exception)
        {
        }
    }
}