using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni address details response
    /// </summary>
    public class OmniAddressDetailsResponse
    {
        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// balance
        /// </summary>
        [JsonProperty("balance")]
        public OmniAssetBalanceJson[] Balances { get; set; }

        /// <summary>
        /// transactions
        /// </summary>
        [JsonProperty("transactions")]
        public OmniTransactionJson[] Transactions { get; set; }

        /// <summary>
        /// pages
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; set; }

        /// <summary>
        /// txcount
        /// </summary>
        [JsonProperty("txcount")]
        public int TxCount { get; set; }
    }
}
