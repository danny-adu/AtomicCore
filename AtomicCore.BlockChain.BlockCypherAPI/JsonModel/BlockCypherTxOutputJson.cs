using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#txoutput
    /// </summary>
    public class BlockCypherTxOutputJson
    {
        /// <summary>
        /// Value in this transaction output, in satoshis.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// Raw hexadecimal encoding of the encumbrance script for this output.
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// Addresses that correspond to this output; typically this will only have a single address, and you can think of this output as having "sent" value to the address contained herein.
        /// </summary>
        [JsonProperty("addresses")]
        public string[] Addresses { get; set; }

        /// <summary>
        /// The type of encumbrance script used for this output.
        /// </summary>
        [JsonProperty("script_type")]
        public string ScriptType { get; set; }

        /// <summary>
        /// Optional The transaction hash that spent this output. Only returned for outputs that have been spent. The spending transaction may be unconfirmed.
        /// </summary>
        [JsonProperty("spent_by")]
        public string SpentBy { get; set; }

        /// <summary>
        /// Optional A hex-encoded representation of an OP_RETURN data output, without any other script instructions. Only returned for outputs whose script_type is null-data.
        /// </summary>
        [JsonProperty("data_hex")]
        public string DataHex { get; set; }

        /// <summary>
        /// Optional An ASCII representation of an OP_RETURN data output, without any other script instructions. Only returned for outputs whose script_type is null-data and if its data falls into the visible ASCII range.
        /// </summary>
        [JsonProperty("data_string")]
        public string DataString { get; set; }

       
    }
}
