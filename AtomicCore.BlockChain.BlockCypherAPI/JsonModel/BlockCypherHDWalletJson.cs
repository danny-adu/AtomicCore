using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#hdwallet
    /// </summary>
    public class BlockCypherHDWalletJson
    {
        /// <summary>
        /// User token associated with this HD wallet.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Name of the HD wallet.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// List of HD chains associated with this wallet, each containing HDAddresses. A single chain is returned if the wallet has no subchains.
        /// </summary>
        [JsonProperty("chains")]
        public List<BlockCypherHDChainJson> Chains { get; set; }

        /// <summary>
        /// true for HD wallets, not present for normal wallets.
        /// </summary>
        [JsonProperty("hd")]
        public bool Hd { get; set; }

        /// <summary>
        /// The extended public key all addresses in the HD wallet are derived from. It's encoded in BIP32 format
        /// </summary>
        [JsonProperty("extended_public_key")]
        public string ExtendedPublicKey { get; set; }

        /// <summary>
        /// optional returned for HD wallets created with subchains.
        /// </summary>
        [JsonProperty("subchain_indexes")]
        public int[] SubchainIndexes { get; set; }

        
    }
}
