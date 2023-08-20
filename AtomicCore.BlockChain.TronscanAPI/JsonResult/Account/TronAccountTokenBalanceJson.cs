using Newtonsoft.Json;
using System.Numerics;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Account Token Balance Json
    /// </summary>
    public class TronAccountTokenBalanceJson
    {
        /// <summary>
        /// converted trx value amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// token price intrx
        /// </summary>
        [JsonProperty("tokenPriceInTrx")]
        public decimal TokenPriceInTrx { get; set; }

        /// <summary>
        /// Token ID
        /// </summary>
        [JsonProperty("tokenId")]
        public string TokenID { get; set; }

        /// <summary>
        /// account balance
        /// </summary>
        [JsonProperty("balance")]
        public BigInteger Balance { get; set; }

        /// <summary>
        /// Token Name
        /// </summary>
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }

        /// <summary>
        /// Token Decimals
        /// </summary>
        [JsonProperty("tokenDecimal")]
        public int TokenDecimal { get; set; }

        /// <summary>
        /// Token Abbr
        /// </summary>
        [JsonProperty("tokenAbbr")]
        public string TokenAbbr { get; set; }

        /// <summary>
        /// Token Can Show
        /// </summary>
        [JsonProperty("tokenCanShow")]
        public int TokenCanShow { get; set; }

        /// <summary>
        /// Token Type
        /// </summary>
        [JsonProperty("tokenType")]
        public string TokenType { get; set; }

        /// <summary>
        /// vip
        /// </summary>
        [JsonProperty("vip")]
        public bool IsVip { get; set; }

        /// <summary>
        /// token logo
        /// </summary>
        [JsonProperty("tokenLogo")]
        public string TokenLogo { get; set; }
    }
}
