﻿// Copyright (c) 2015 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetChainTipsResponse
    {
        public long Height { get; set; }
        public string Hash { get; set; }
        public int BranchLen { get; set; }
        public string Status { get; set; }
    }
}
