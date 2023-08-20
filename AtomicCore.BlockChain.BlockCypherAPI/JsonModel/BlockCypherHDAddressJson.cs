using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#hdaddress
    /// </summary>
    public class BlockCypherHDAddressJson
    {
        /// <summary>
        /// Standard address representation.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The BIP32 path of the HD address.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// optional Contains the hex-encoded public key if returned by Derive Address in Wallet endpoint.
        /// </summary>
        [JsonProperty("public")]
        public string Public { get; set; }
    }
}
