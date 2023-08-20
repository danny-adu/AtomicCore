using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/?shell#address
    /// </summary>
    public class BlockCypherAddressJson
    {
        /// <summary>
        /// Optional The requested address. Not returned if querying a wallet/HD wallet.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional The requested wallet object. Only returned if querying by wallet name instead of public address.
        /// </summary>
        [JsonProperty("wallet")]
        public BlockCypherWalletJson Wallet { get; set; }

        /// <summary>
        /// Optional The requested HD wallet object. Only returned if querying by HD wallet name instead of public address.
        /// </summary>
        [JsonProperty("hd_wallet")]
        public BlockCypherHDWalletJson HdWallet { get; set; }

        /// <summary>
        /// Total amount of confirmed satoshis received by this address.
        /// </summary>
        [JsonProperty("total_received")]
        public int TotalReceived { get; set; }

        /// <summary>
        /// Total amount of confirmed satoshis sent by this address.
        /// </summary>
        [JsonProperty("total_sent")]
        public int TotalSent { get; set; }

        /// <summary>
        /// Balance of confirmed satoshis on this address. This is the difference between outputs and inputs on this address, but only for transactions that have been included into a block (i.e., for transactions whose confirmations > 0).
        /// </summary>
        [JsonProperty("balance")]
        public int Balance { get; set; }

        /// <summary>
        /// Balance of unconfirmed satoshis on this address. Can be negative (if unconfirmed transactions are just spending outputs). Only unconfirmed transactions (haven't made it into a block) are included.
        /// </summary>
        [JsonProperty("unconfirmed_balance")]
        public int UnconfirmedBalance { get; set; }

        /// <summary>
        /// Total balance of satoshis, including confirmed and unconfirmed transactions, for this address.
        /// </summary>
        [JsonProperty("final_balance")]
        public int FinalBalance { get; set; }

        /// <summary>
        /// Number of confirmed transactions on this address. Only transactions that have made it into a block (confirmations > 0) are counted.
        /// </summary>
        [JsonProperty("n_tx")]
        public int NTx { get; set; }

        /// <summary>
        /// Number of unconfirmed transactions for this address. Only unconfirmed transactions (confirmations == 0) are counted.
        /// </summary>
        [JsonProperty("unconfirmed_n_tx")]
        public int UnconfirmedNTx { get; set; }

        /// <summary>
        /// Final number of transactions, including confirmed and unconfirmed transactions, for this address.
        /// </summary>
        [JsonProperty("final_n_tx")]
        public int FinalNTx { get; set; }

        /// <summary>
        /// Optional To retrieve base URL transactions. To get the full URL, concatenate this URL with a transaction's hash.
        /// </summary>
        [JsonProperty("tx_url")]
        public string TxUrl { get; set; }

        /// <summary>
        /// Optional Array of full transaction details associated with this address. Usually only returned from the Address Full Endpoint.
        /// </summary>
        [JsonProperty("txs")]
        public List<BlockCypherTxJson> Txs { get; set; }

        /// <summary>
        /// Optional Array of transaction inputs and outputs for this address. Usually only returned from the standard Address Endpoint.
        /// </summary>
        [JsonProperty("txrefs")]
        public List<BlockCypherTxRefJson> Txrefs { get; set; }

        /// <summary>
        /// Optional All unconfirmed transaction inputs and outputs for this address. Usually only returned from the standard Address Endpoint.
        /// </summary>
        [JsonProperty("unconfirmed_txrefs")]
        public List<BlockCypherTxRefJson> UnconfirmedTxrefs { get; set; }

        /// <summary>
        /// Optional If true, then the Address object contains more transactions than shown. Useful for determining whether to poll the API for more transaction information.
        /// </summary>
        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }
    }
}
