using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#addresskeychain
    /// </summary>
    public class BlockCypherAddressKeychainJson
    {
        /// <summary>
        /// Standard address representation.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Hex-encoded Public key.
        /// </summary>
        [JsonProperty("public")]
        public string Public { get; set; }

        /// <summary>
        /// Hex-encoded Private key.
        /// </summary>
        [JsonProperty("private")]
        public string Private { get; set; }

        /// <summary>
        /// Wallet import format, a common encoding for the private key.
        /// </summary>
        [JsonProperty("wif")]
        public string Wif { get; set; }

        /// <summary>
        /// Optional Array of public keys to provide to generate a multisig address.
        /// </summary>
        [JsonProperty("pubkeys")]
        public string[] Pubkeys { get; set; }

        /// <summary>
        /// Optional If generating a multisig address, the type of multisig script; typically "multisig-n-of-m", where n and m are integers.
        /// </summary>
        [JsonProperty("script_type")]
        public string ScriptType { get; set; }

        /// <summary>
        /// Optional If generating an OAP address, this represents the parent blockchain's underlying address (the typical address listed above).
        /// </summary>
        [JsonProperty("original_address")]
        public string OriginalAddress { get; set; }

        /// <summary>
        /// Optional The OAP address, if generated using the Generate Asset Address Endpoint.
        /// </summary>
        [JsonProperty("oap_address")]
        public string OapAddress { get; set; }

       
    }
}
