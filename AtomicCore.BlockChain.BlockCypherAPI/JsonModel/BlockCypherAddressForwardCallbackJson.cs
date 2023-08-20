using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#addressforwardcallback
    /// </summary>
    public class BlockCypherAddressForwardCallbackJson
    {
        /// <summary>
        /// Amount sent to the destination address, in satoshis.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// The intermediate address to which the transaction was originally sent.
        /// </summary>
        [JsonProperty("input_address")]
        public string InputAddress { get; set; }

        /// <summary>
        /// The final destination address to which the forward will eventually be sent.
        /// </summary>
        [JsonProperty("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The transaction hash representing the initial transaction to the input_address.
        /// </summary>
        [JsonProperty("input_transaction_hash")]
        public string InputTransactionHash { get; set; }

        /// <summary>
        /// The transaction hash of the generated transaction that forwards from the input_address to the destination.
        /// </summary>
        [JsonProperty("transaction_hash")]
        public string TransactionHash { get; set; }

        
    }
}
