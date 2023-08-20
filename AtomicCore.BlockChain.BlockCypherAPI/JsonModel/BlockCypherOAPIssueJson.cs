using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#oapissue
    /// </summary>
    public class BlockCypherOAPIssueJson
    {
        /// <summary>
        /// The private key being used to issue or transfer assets.
        /// </summary>
        [JsonProperty("from_private")]
        public string FromPrivate { get; set; }

        /// <summary>
        /// The target OAP address assets for issue or transfer.
        /// </summary>
        [JsonProperty("to_address")]
        public string ToAddress { get; set; }

        /// <summary>
        /// The amount of assets being issued or transfered.
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Optional Hex-encoded metadata that can optionally be encoded into the issue or transfer transaction.
        /// </summary>
        [JsonProperty("metadata")]
        public string Metadata { get; set; }
    }
}
