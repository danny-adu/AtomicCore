using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#witnesstosigntx
    /// </summary>
    public class BlockCypherWitnessToSignTxJson
    {
        /// <summary>
        /// Version of the transaction.
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// Hash of the previous output.
        /// </summary>
        [JsonProperty("hash_prevouts")]
        public string HashPrevouts { get; set; }

        /// <summary>
        /// Hash sequence.
        /// </summary>
        [JsonProperty("hash_sequence")]
        public string HashSequence { get; set; }

        /// <summary>
        /// Hash of the outpoint.
        /// </summary>
        [JsonProperty("outpoint")]
        public string Outpoint { get; set; }

        /// <summary>
        /// Outpoint index.
        /// </summary>
        [JsonProperty("outpoint_index")]
        public int OutpointIndex { get; set; }

        /// <summary>
        /// Script code of the input.
        /// </summary>
        [JsonProperty("script_code")]
        public string ScriptCode { get; set; }

        /// <summary>
        /// Value of the output spent by this input.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// Sequence number of the input.
        /// </summary>
        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// Hash of the output.
        /// </summary>
        [JsonProperty("hash_outputs")]
        public string HashOutputs { get; set; }

        /// <summary>
        /// Lock time of the transaction.
        /// </summary>
        [JsonProperty("lock_time")]
        public int LockTime { get; set; }

        /// <summary>
        /// sighash type of the signature.
        /// </summary>
        [JsonProperty("sighash_type")]
        public int SighashType { get; set; }
        
    }
}
