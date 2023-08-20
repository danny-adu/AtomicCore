// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// SigHashType
    /// </summary>
    public static class SigHashType
    {
        /// <summary>
        /// ALL
        /// </summary>
        public const string All = "ALL";

        /// <summary>
        /// NONE
        /// </summary>
        public const string None = "NONE";

        /// <summary>
        /// SINGLE
        /// </summary>
        public const string Single = "SINGLE";

        /// <summary>
        /// ALL|ANYONECANPAY
        /// </summary>
        public const string AllAnyoneCanPay = "ALL|ANYONECANPAY";

        /// <summary>
        /// NONE|ANYONECANPAY
        /// </summary>
        public const string NoneAnyoneCanPay = "NONE|ANYONECANPAY";

        /// <summary>
        /// SINGLE|ANYONECANPAY
        /// </summary>
        public const string SingleAnyoneCanPay = "SINGLE|ANYONECANPAY";
    }
}