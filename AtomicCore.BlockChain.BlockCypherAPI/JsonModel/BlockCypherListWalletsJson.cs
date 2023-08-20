using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#list-wallets-endpoint
    /// List Wallets Endpoint 返回
    /// </summary>
    public class BlockCypherListWalletsJson
    {
        /// <summary>
        /// WalletNames
        /// </summary>
        [JsonProperty("wallet_names")]
        public string WalletNames { get; set; }
    }
}
