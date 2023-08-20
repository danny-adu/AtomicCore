// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// Coin Constants
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CoinConstants<T> where T : CoinConstants<T>, new()
    {
        /// <summary>
        /// lazy list
        /// </summary>
        private static readonly Lazy<T> Lazy = new Lazy<T>(() => new T());

        /// <summary>
        /// instance
        /// </summary>
        public static T Instance
        {
            get { return Lazy.Value; }
        }
    }
}