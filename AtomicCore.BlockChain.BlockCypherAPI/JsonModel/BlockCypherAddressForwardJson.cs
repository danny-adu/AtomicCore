using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#addressforward
    /// </summary>
    public class BlockCypherAddressForwardJson
    {
        /// <summary>
        /// Identifier of the orwarding request; generated when a new request is created..
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// The mandatory user token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// The required destination address to forward to.
        /// </summary>
        [JsonProperty("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// The address which will automatically forward to destination; generated when a new request is created.
        /// </summary>
        [JsonProperty("input_address")]
        public string InputAddress { get; set; }

        /// <summary>
        /// Optional Address to forward processing fees, if specified. Allows you to receive a fee for your own services.
        /// </summary>
        [JsonProperty("process_fees_address")]
        public string ProcessFeesAddress { get; set; }

        /// <summary>
        /// Optional Fixed processing fee amount to be sent to the fee address. A fixed satoshi amount or a percentage is required if a process_fees_address has been specified.
        /// </summary>
        [JsonProperty("process_fees_satoshis")]
        public int ProcessFeesSatoshis { get; set; }

        /// <summary>
        /// Optional Percentage of the transaction to be sent to the fee address. A fixed satoshi amount or a percentage is required if a process_fees_address has been specified.
        /// </summary>
        [JsonProperty("process_fees_percent")]
        public float ProcessFeesPercent { get; set; }

        /// <summary>
        /// Optional The URL to call anytime a new transaction is forwarded.
        /// </summary>
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Optional Whether to also call the callback_url with subsequent confirmations of the forwarding transactions. Automatically sets up a WebHook.
        /// </summary>
        [JsonProperty("enable_confirmations")]
        public bool EnableConfirmations { get; set; }

        /// <summary>
        /// Optional Mining fee amount to include in the forwarding transaction, in satoshis. If not set, defaults to 10,000.
        /// </summary>
        [JsonProperty("mining_fees_satoshis")]
        public int MiningFeesSatoshis { get; set; }

        /// <summary>
        /// Optional History of forwarding transaction hashes for this forward; not present if this request has yet to forward any transactions.
        /// </summary>
        [JsonProperty("txs")]
        public string[] Txs { get; set; }

      
    }
}
