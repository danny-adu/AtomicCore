// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// RawTransaction Invalid Amount Exception
    /// </summary>
    [Serializable]
    public class RawTransactionInvalidAmountException : Exception
    {
        /// <summary>
        /// RawTransactionInvalidAmountException
        /// </summary>
        public RawTransactionInvalidAmountException() : base("Raw Transaction amount is invalid.")
        {
        }

        /// <summary>
        /// RawTransactionInvalidAmountException
        /// </summary>
        /// <param name="message"></param>
        public RawTransactionInvalidAmountException(string message) : base(message)
        {
        }

        /// <summary>
        /// RawTransactionInvalidAmountException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RawTransactionInvalidAmountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}