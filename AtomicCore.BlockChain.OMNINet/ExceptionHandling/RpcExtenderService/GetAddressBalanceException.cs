// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// GetAddressBalanceException
    /// </summary>
    [Serializable]
    public class GetAddressBalanceException : Exception
    {
        /// <summary>
        /// GetAddressBalanceException
        /// </summary>
        public GetAddressBalanceException()
        {
        }

        /// <summary>
        /// GetAddressBalanceException
        /// </summary>
        /// <param name="customMessage"></param>
        public GetAddressBalanceException(string customMessage) : base(customMessage)
        {
        }

        /// <summary>
        /// GetAddressBalanceException
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public GetAddressBalanceException(string customMessage, Exception exception) : base(customMessage, exception)
        {
        }
    }
}