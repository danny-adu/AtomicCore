﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using System;

namespace AtomicCore.BlockChain.OMNINet
{
    public class SignRawTransactionResponse
    {
        public string Hex { get; set; }
        public bool Complete { get; set; }
    }
}