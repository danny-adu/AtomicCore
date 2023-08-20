﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// RpcException
    /// </summary>
    [Serializable]
    public class RpcException : Exception
    {
        /// <summary>
        /// RpcException
        /// </summary>
        public RpcException() { }

        /// <summary>
        /// RpcException
        /// </summary>
        /// <param name="customMessage"></param>
        public RpcException(string customMessage) : base(customMessage)
        {
        }

        /// <summary>
        /// RpcException
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public RpcException(string customMessage, Exception exception) : base(customMessage, exception)
        {
        }
    }
}