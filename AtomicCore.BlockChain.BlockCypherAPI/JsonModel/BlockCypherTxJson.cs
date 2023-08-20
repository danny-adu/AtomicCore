using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// block cypher tx json
    /// https://www.blockcypher.com/dev/bitcoin/?shell#tx
    /// </summary>
    public class BlockCypherTxJson
    {
        /// <summary>
        /// Height of the block that contains this transaction. If this is an unconfirmed transaction, it will equal -1.
        /// </summary>
        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        /// <summary>
        /// The hash of the transaction. While reasonably unique, using hashes as identifiers may be unsafe.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Array of bitcoin public addresses involved in the transaction.
        /// </summary>
        [JsonProperty("addresses")]
        public string[] Addresses { get; set; }

        /// <summary>
        /// The total number of satoshis exchanged in this transaction.
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        /// The total number of fees---in satoshis---collected by miners in this transaction.
        /// </summary>
        [JsonProperty("fees")]
        public int Fees { get; set; }

        /// <summary>
        /// The size of the transaction in bytes.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// The virtual size of the transaction in bytes.
        /// </summary>
        [JsonProperty("vsize")]
        public int Vsize { get; set; }

        /// <summary>
        /// The likelihood that this transaction will make it to the next block; reflects the preference level miners have to include this transaction. Can be high, medium or low.
        /// </summary>
        [JsonProperty("preference")]
        public string Preference { get; set; }

        /// <summary>
        /// Address of the peer that sent BlockCypher's servers this transaction.
        /// </summary>
        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        /// <summary>
        /// Time this transaction was received by BlockCypher's servers.
        /// </summary>
        [JsonProperty("received")]
        public DateTime Received { get; set; }

        /// <summary>
        /// Version number, typically 1 for Bitcoin transactions.
        /// </summary>
        [JsonProperty("ver")]
        public int Ver { get; set; }

        /// <summary>
        /// Time when transaction can be valid. Can be interpreted in two ways: if less than 500 million, refers to block height. If more, refers to Unix epoch time.
        /// </summary>
        [JsonProperty("lock_time")]
        public int LockTime { get; set; }

        /// <summary>
        /// true if this is an attempted double spend; false otherwise.
        /// </summary>
        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        /// <summary>
        /// Total number of inputs in the transaction.
        /// </summary>
        [JsonProperty("vin_sz")]
        public int VinSz { get; set; }

        /// <summary>
        /// Total number of outputs in the transaction.
        /// </summary>
        [JsonProperty("vout_sz")]
        public int VoutSz { get; set; }

        /// <summary>
        /// Number of subsequent blocks, including the block the transaction is in. Unconfirmed transactions have 0 confirmations.
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// TXInput Array, limited to 20 by default.
        /// </summary>
        [JsonProperty("inputs")]
        public List<BlockCypherTxInputJson> Inputs { get; set; }

        /// <summary>
        /// TXOutput Array, limited to 20 by default.
        /// </summary>
        [JsonProperty("outputs")]
        public List<BlockCypherTxOutputJson> Outputs { get; set; }

        /// <summary>
        /// Optional Returns true if this transaction has opted in to Replace-By-Fee (RBF), either true or not present. You can read more about Opt-In RBF here.
        /// </summary>
        [JsonProperty("opt_in_rbf")]
        public bool OptInRbf { get; set; }

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
        /// Optional Number of peers that have sent this transaction to BlockCypher; only present for unconfirmed transactions.
        /// </summary>
        [JsonProperty("receive_count")]
        public int ReceiveCount { get; set; }

        /// <summary>
        /// Optional Address BlockCypher will use to send back your change, if you constructed this transaction. If not set, defaults to the address from which the coins were originally sent.
        /// </summary>
        [JsonProperty("change_address")]
        public string ChangeAddress { get; set; }

        /// <summary>
        /// Optional Hash of the block that contains this transaction; only present for confirmed transactions.
        /// </summary>
        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// Optional Canonical, zero-indexed location of this transaction in a block; only present for confirmed transactions.
        /// </summary>
        [JsonProperty("block_index")]
        public int BlockIndex { get; set; }

        /// <summary>
        /// Optional If this transaction is a double-spend (i.e. double_spend == true) then this is the hash of the transaction it's double-spending.
        /// </summary>
        [JsonProperty("double_of")]
        public string DoubleOf { get; set; }

        /// <summary>
        /// Optional Returned if this transaction contains an OP_RETURN associated with a known data protocol. Data protocols currently detected: blockchainid ; openassets ; factom ; colu ; coinspark ; omni
        /// </summary>
        [JsonProperty("data_protocol")]
        public string DataProtocol { get; set; }

        /// <summary>
        /// Optional Hex-encoded bytes of the transaction, as sent over the network.
        /// </summary>
        [JsonProperty("hex")]
        public string Hex { get; set; }

        /// <summary>
        /// Optional If there are more transaction inptus that couldn't fit into the TXInput array, this is the BlockCypher URL to query the next set of TXInputs (within a TX object).
        /// </summary>
        [JsonProperty("next_inputs")]
        public string NextInputs { get; set; }

        /// <summary>
        /// Optional If there are more transaction outputs that couldn't fit into the TXOutput array, this is the BlockCypher URL to query the next set of TXOutputs(within a TX object).
        /// </summary>
        [JsonProperty("next_outputs")]
        public string NextOutputs { get; set; }

       
    }
}
