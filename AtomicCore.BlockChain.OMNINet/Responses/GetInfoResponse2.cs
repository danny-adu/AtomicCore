using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomicCore.BlockChain.OMNINet
{
    public class GetInfoResponse2
    {
        public string version { get; set; }

        public string protocolversion { get; set; }

        public string walletversion { get; set; }

        public double balance { get; set; }

        public double newmint { get; set; }

        public double stake { get; set; }

        public uint blocks { get; set; }

        public double moneysupply { get; set; }

        public uint connections { get; set; }

        public string proxy { get; set; }

        public string ip { get; set; }

        public double difficulty { get; set; }

        public bool testnet { get; set; }

        public long keypoololdest { get; set; }

        public int keypoolsize { get; set; }

        public double paytxfee { get; set; }

        public string errors { get; set; }
    }
}
