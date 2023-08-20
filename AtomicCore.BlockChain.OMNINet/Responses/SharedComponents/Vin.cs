﻿// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    public class Vin
    {
        public string TxId { get; set; }
        public int Vout { get; set; }
        public ScriptSig ScriptSig { get; set; }
        public string CoinBase { get; set; }
        public string[] TxInWitness { get; set; }
        public string Sequence { get; set; }
    }

    public class ScriptSig
    {
        public string Asm { get; set; }
        public string Hex { get; set; }
    }
}
