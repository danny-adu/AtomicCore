using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#txconfidence
    /// </summary>
    public class BlockCypherTxConfidenceJson
    {
        /// <summary>
        /// The age of the transaction in milliseconds, based on the earliest time BlockCypher saw it relayed in the network.
        /// </summary>
        [JsonProperty("age_millis")]
        public int AgeMillis { get; set; }

        /// <summary>
        /// Number of peers that have sent this transaction to BlockCypher; only positive for unconfirmed transactions. -1 for confirmed transactions.
        /// </summary>
        [JsonProperty("receive_count")]
        public int ReceiveCount { get; set; }

        /// <summary>
        /// A number from 0 to 1 representing BlockCypher's confidence that the transaction won't be double-spent against.
        /// </summary>
        [JsonProperty("confidence")]
        public float Confidence { get; set; }

        /// <summary>
        /// The hash of the transaction. While reasonably unique, using hashes as identifiers may be unsafe.
        /// </summary>
        [JsonProperty("txhash")]
        public string Txhash { get; set; }

        /// <summary>
        /// The BlockCypher URL one can use to query more detailed information about this transaction.
        /// </summary>
        [JsonProperty("txurl")]
        public string Txurl { get; set; }
       
    }
}
