using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#event
    /// </summary>
    public class BlockCypherEventJson
    {
        /// <summary>
        /// Identifier of the event; generated when a new request is created.
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Type of event; can be unconfirmed-tx, new-block, confirmed-tx, tx-confirmation, double-spend-tx, tx-confidence.
        /// </summary>
        [JsonProperty("event")]
        public string Event { get; set; }

        /// <summary>
        /// optional Only objects with a matching hash will be sent. The hash can either be for a block or a transaction.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// optional Only transactions associated with the given wallet will be sent; can use a regular or HD wallet name. If used, requires a user token.
        /// </summary>
        [JsonProperty("wallet_name")]
        public string WalletName { get; set; }

        /// <summary>
        /// optional Required if wallet_name is used, though generally we advise users to include it (as they can reach API throttling thresholds rapidly).
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// optional Only transactions associated with the given address will be sent. A wallet name can also be used instead of an address, which will then match on any address in the wallet.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// optional Used in concert with the tx-confirmation event type to set the number of confirmations desired for which to receive an update. You'll receive an updated TX for every confirmation up to this amount. The maximum allowed is 10; if not set, it will default to 6.
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// optional Used in concert with the tx-confidence event type to set the minimum confidence for which you'll receive a notification. You'll receive a TX once this threshold is met. Will accept any float between 0 and 1, exclusive; if not set, defaults to 0.99.
        /// </summary>
        [JsonProperty("confidence")]
        public float Confidence { get; set; }

        /// <summary>
        /// optional Only transactions with an output script of the provided type will be sent. The recognized types of scripts are: pay-to-pubkey-hash, pay-to-multi-pubkey-hash, pay-to-pubkey, pay-to-script-hash, null-data (sometimes called OP_RETURN), empty or unknown.
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// optional Callback URL for this Event's WebHook; not applicable for WebSockets usage.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Number of errors when attempting to POST to callback URL; not applicable for WebSockets usage.
        /// </summary>
        [JsonProperty("callback_errors")]
        public string CallbackErrors { get; set; }

       
    }
}
