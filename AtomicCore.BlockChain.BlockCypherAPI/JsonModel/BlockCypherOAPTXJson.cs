using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#oaptx
    /// </summary>
    public class BlockCypherOAPTXJson
    {
        /// <summary>
        /// Version of Open Assets Protocol transaction. Typically 1.
        /// </summary>
        [JsonProperty("ver")]
        public int Ver { get; set; }

        /// <summary>
        /// Unique indentifier associated with this asset; can be used to query other transactions associated with this asset.
        /// </summary>
        [JsonProperty("assetid")]
        public string Assetid { get; set; }

        /// <summary>
        /// This transaction's unique hash; same as the underlying transaction on the asset's parent blockchain.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Optional Time this transaction was confirmed; only returned for confirmed transactions.
        /// </summary>
        [JsonProperty("confirmed")]
        public DateTime Confirmed { get; set; }

        /// <summary>
        /// Time this transaction was received.
        /// </summary>
        [JsonProperty("received")]
        public DateTime Received { get; set; }

        /// <summary>
        /// Optional Associated hex-encoded metadata with this transaction, if it exists.
        /// </summary>
        [JsonProperty("oap_meta")]
        public string OapMeta { get; set; }

        /// <summary>
        /// true if this is an attempted double spend; false otherwise.
        /// </summary>
        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        /// <summary>
        /// Array of input data, which can be seen explicitly in the cURL example. Very similar to array of TXInputs, but with values related to assets instead of satoshis.
        /// </summary>
        [JsonProperty("inputs")]
        public List<BlockCypherTxInputJson> Inputs { get; set; }

        /// <summary>
        /// Array of output data, which can be seen explicitly in the cURL example. Very similar to array of TXOutputs, but with values related to assets instead of satoshis.
        /// </summary>
        [JsonProperty("outputs")]
        public List<BlockCypherTxOutputJson> Outputs { get; set; }

        
    }
}
