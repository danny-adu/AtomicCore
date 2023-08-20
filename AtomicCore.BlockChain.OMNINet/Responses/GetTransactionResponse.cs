﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.OMNINet
{
    //  Note: Local wallet transactions only
    public class GetTransactionResponse : ITransactionResponse
    {
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string BlockHash { get; set; }
        public int BlockIndex { get; set; }
        public int BlockTime { get; set; }
        public int Confirmations { get; set; }
        public List<GetTransactionResponseDetails> Details { get; set; }
        public string Hex { get; set; }
        public int Time { get; set; }
        public int TimeReceived { get; set; }
        public List<string> WalletConflicts { get; set; }
        public string TxId { get; set; }
    }

    public class GetTransactionResponseDetails
    {
        public string Account { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public int Vout { get; set; }
        public string Category { get; set; }
    }
}