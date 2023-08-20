using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#wallet
    /// </summary>
    public class BlockCypherWalletJson
    {
        /// <summary>
        /// User token associated with this wallet.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Name of the wallet.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// List of addresses associated with this wallet.
        /// </summary>
        [JsonProperty("addresses")]
        public string[] Addresses { get; set; }

       
    }
}
