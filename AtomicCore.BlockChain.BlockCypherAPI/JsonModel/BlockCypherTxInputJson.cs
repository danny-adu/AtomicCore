using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#txinput
    /// </summary>
    public class BlockCypherTxInputJson
    {
        /// <summary>
        /// The previous transaction hash where this input was an output. Not present for coinbase transactions.
        /// </summary>
        [JsonProperty("prev_hash")]
        public string PrevHash { get; set; }

        /// <summary>
        /// The index of the output being spent within the previous transaction. Not present for coinbase transactions.
        /// </summary>
        [JsonProperty("output_index")]
        public int OutputIndex { get; set; }

        /// <summary>
        /// The value of the output being spent within the previous transaction. Not present for coinbase transactions.
        /// </summary>
        [JsonProperty("output_value")]
        public int OutputValue { get; set; }

        /// <summary>
        /// The type of script that encumbers the output corresponding to this input.
        /// </summary>
        [JsonProperty("script_type")]
        public string ScriptType { get; set; }

        /// <summary>
        /// Raw hexadecimal encoding of the script.
        /// </summary>
        [JsonProperty("script")]
        public string Script { get; set; }

        /// <summary>
        /// An array of public addresses associated with the output of the previous transaction.
        /// </summary>
        [JsonProperty("addresses")]
        public string[] Addresses { get; set; }

        /// <summary>
        /// Legacy 4-byte sequence number, not usually relevant unless dealing with locktime encumbrances.
        /// </summary>
        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// Optional Number of confirmations of the previous transaction for which this input was an output. Currently, only returned in unconfirmed transactions.
        /// </summary>
        [JsonProperty("age")]
        public int Age { get; set; }

        /// <summary>
        /// Optional Name of Wallet or HDWallet from which to derive inputs. Only used when constructing transactions via the Creating Transactions process.
        /// </summary>
        [JsonProperty("wallet_name")]
        public string WalletName { get; set; }

        /// <summary>
        /// Optional Token associated with Wallet or HDWallet used to derive inputs. Only used when constructing transactions via the Creating Transactions process.
        /// </summary>
        [JsonProperty("wallet_token")]
        public string WalletToken { get; set; }

       
    }
}
