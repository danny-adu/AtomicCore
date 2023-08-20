using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#txskeleton
    /// </summary>
    public class BlockCypherTxSkeletonJson
    {
        /// <summary>
        /// A temporary TX, usually returned fully filled but missing input scripts.
        /// </summary>
        [JsonProperty("tx")]
        public BlockCypherTxJson Tx { get; set; }

        /// <summary>
        /// Array of hex-encoded data for you to sign, one for each input.
        /// </summary>
        [JsonProperty("tosign")]
        public string[] Tosign { get; set; }

        /// <summary>
        /// Array of signatures corresponding to all the data in tosign, typically provided by you.
        /// </summary>
        [JsonProperty("signatures")]
        public string[] Signatures { get; set; }

        /// <summary>
        /// Array of public keys corresponding to each signature. In general, these are provided by you, and correspond to the signatures you provide.
        /// </summary>
        [JsonProperty("pubkeys")]
        public string[] Pubkeys { get; set; }

        /// <summary>
        /// Optional Array of hex-encoded, work-in-progress transactions; optionally returned to validate the tosign data locally.
        /// </summary>
        [JsonProperty("tosign_tx")]
        public string[] TosignTx { get; set; }

        /// <summary>
        /// Optional Array of errors in the form "error":"description-of-error". This is only returned if there was an error in any stage of transaction generation, and is usually accompanied by a HTTP 400 code.
        /// </summary>
        [JsonProperty("errors")]
        public string[] Errors { get; set; }

       
    }
}
