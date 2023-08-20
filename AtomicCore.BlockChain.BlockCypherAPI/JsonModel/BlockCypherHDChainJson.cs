using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#hdchain
    /// </summary>
    public class BlockCypherHDChainJson
    {
        /// <summary>
        /// Array of HDAddresses associated with this subchain.
        /// </summary>
        [JsonProperty("chain_addresses")]
        public List<BlockCypherHDAddressJson> ChainAddresses { get; set; }

        /// <summary>
        /// optional Index of the subchain, returned if the wallet has subchains.
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

    }
}
