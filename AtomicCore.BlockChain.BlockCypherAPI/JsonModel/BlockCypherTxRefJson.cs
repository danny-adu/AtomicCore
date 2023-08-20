using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#txref
    /// </summary>
    public class BlockCypherTxRefJson
    {
        /// <summary>
        /// Optional The address associated with this transaction input/output. Only returned when querying an address endpoint via a wallet/HD wallet name.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Height of the block that contains this transaction input/output. If it's unconfirmed, this will equal -1.
        /// </summary>
        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        /// <summary>
        /// The hash of the transaction containing this input/output. While reasonably unique, using hashes as identifiers may be unsafe.
        /// </summary>
        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Index of this input in the enclosing transaction. It's a negative number for an output.
        /// </summary>
        [JsonProperty("tx_input_n")]
        public int TxInputN { get; set; }

        /// <summary>
        /// Index of this output in the enclosing transaction. It's a negative number for an input.
        /// </summary>
        [JsonProperty("tx_output_n")]
        public int TxOutputN { get; set; }

        /// <summary>
        /// The value transfered by this input/output in satoshis exchanged in the enclosing transaction.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// The likelihood that the enclosing transaction will make it to the next block; reflects the preference level miners have to include the enclosing transaction. Can be high, medium or low.
        /// </summary>
        [JsonProperty("preference")]
        public string Preference { get; set; }

        /// <summary>
        /// true if this is an output and was spent. If it's an input, or an unspent output, it will be false.
        /// </summary>
        [JsonProperty("spent")]
        public bool Spent { get; set; }

        /// <summary>
        /// true if this is an attempted double spend; false otherwise.
        /// </summary>
        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        /// <summary>
        /// Number of subsequent blocks, including the block the transaction is in. Unconfirmed transactions have 0 confirmations.
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// Optional Raw, hex-encoded script of this input/output.
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// Optional The past balance of the parent address the moment this transaction was confirmed. Not present for unconfirmed transactions.
        /// </summary>
        [JsonProperty("ref_balance")]
        public int RefBalance { get; set; }

        /// <summary>
        /// Optional The percentage chance this transaction will not be double-spent against, if unconfirmed. For more information, check the section on Confidence Factor.
        /// </summary>
        [JsonProperty("confidence")]
        public float Confidence { get; set; }

        /// <summary>
        /// Optional Time at which transaction was included in a block; only present for confirmed transactions.
        /// </summary>
        [JsonProperty("confirmed")]
        public DateTime Confirmed { get; set; }

        /// <summary>
        /// Optional The transaction hash that spent this output. Only returned for outputs that have been spent. The spending transaction may be unconfirmed.
        /// </summary>
        [JsonProperty("spent_by")]
        public string SpentBy { get; set; }

        /// <summary>
        /// Optional Time this transaction was received by BlockCypher's servers; only present for unconfirmed transactions.
        /// </summary>
        [JsonProperty("received")]
        public DateTime Received { get; set; }

        /// <summary>
        /// Optional Number of peers that have sent this transaction to BlockCypher; only present for unconfirmed transactions.
        /// </summary>
        [JsonProperty("receive_count")]
        public int ReceiveCount { get; set; }

        /// <summary>
        /// Optional If this transaction is a double-spend (i.e. double_spend == true) then this is the hash of the transaction it's double-spending.
        /// </summary>
        [JsonProperty("double_of")]
        public string DoubleOf { get; set; }

    }
}
