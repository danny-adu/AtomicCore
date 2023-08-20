// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// CreateRawTransactionInput
    /// </summary>
    public class CreateRawTransactionInput
    {
        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string TxId { get; set; }

        /// <summary>
        /// vout
        /// </summary>
        [JsonProperty("vout")]
        public int Vout { get; set; }
    }
}