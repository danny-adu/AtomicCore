﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// Note: This serves as a common interface for the cases that a strongly-typed response is required 
    /// while it is not yet clear whether the transaction in question is in-wallet or not A practical 
    /// example is the bridging of GetTransaction(), DecodeRawTransaction() and GetRawTransaction()
    /// </summary>
    public interface ITransactionResponse
    {
        /// <summary>
        /// TXID
        /// </summary>
        string TxId { get; set; }
    }
}