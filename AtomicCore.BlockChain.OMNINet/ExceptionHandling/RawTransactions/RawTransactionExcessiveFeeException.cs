// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// RawTransaction Excessive Fee Exception
    /// </summary>
    [Serializable]
    public class RawTransactionExcessiveFeeException : Exception
    {
        /// <summary>
        /// RawTransactionExcessiveFeeException
        /// </summary>
        public RawTransactionExcessiveFeeException() : base("Fee in raw transaction is greater than specified amount.")
        {
        }

        /// <summary>
        /// RawTransactionExcessiveFeeException
        /// </summary>
        /// <param name="maxSpecifiedFee"></param>
        public RawTransactionExcessiveFeeException(decimal maxSpecifiedFee) : base(string.Format("Fee in raw transaction is greater than specified amount of {0}.", maxSpecifiedFee))
        {
        }

        /// <summary>
        /// RawTransactionExcessiveFeeException
        /// </summary>
        /// <param name="actualFee"></param>
        /// <param name="maxSpecifiedFee"></param>
        public RawTransactionExcessiveFeeException(decimal actualFee, decimal maxSpecifiedFee) : base(string.Format("Fee of {0} in raw transaction is greater than specified amount of {1}.", actualFee, maxSpecifiedFee))
        {
        }

        /// <summary>
        /// RawTransactionExcessiveFeeException
        /// </summary>
        /// <param name="message"></param>
        public RawTransactionExcessiveFeeException(string message) : base(message)
        {
        }

        /// <summary>
        /// RawTransactionExcessiveFeeException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RawTransactionExcessiveFeeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}