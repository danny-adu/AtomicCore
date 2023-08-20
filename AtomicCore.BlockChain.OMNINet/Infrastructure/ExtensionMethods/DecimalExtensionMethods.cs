// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// DecimalExtensionMethods
    /// </summary>
    public static class DecimalExtensionMethods
    {
        /// <summary>
        /// GetNumberOfDecimalPlaces
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static ushort GetNumberOfDecimalPlaces(this decimal number)
        {
            return BitConverter.GetBytes(decimal.GetBits(number)[3])[2];
        }
    }
}