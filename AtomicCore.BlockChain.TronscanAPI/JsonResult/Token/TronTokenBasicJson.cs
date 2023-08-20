using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Token Info Json
    /// </summary>
    public class TronTokenBasicJson
    {
        /// <summary>
        /// Token ID
        /// </summary>
        [JsonProperty("tokenId")]
        public string TokenID { get; set; }

        /// <summary>
        /// Token Name
        /// </summary>
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }

        /// <summary>
        /// Token Abbr
        /// </summary>
        [JsonProperty("tokenAbbr")]
        public string TokenAbbr { get; set; }

        /// <summary>
        /// Token Decimals
        /// </summary>
        [JsonProperty("tokenDecimal")]
        public int TokenDecimal { get; set; }

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
        /// token logo
        /// </summary>
        [JsonProperty("tokenLogo")]
        public string TokenLogo { get; set; }

        /// <summary>
        /// Token Level
        /// </summary>
        [JsonProperty("tokenLevel")]
        public string TokenLevel { get; set; }

        /// <summary>
        /// vip
        /// </summary>
        [JsonProperty("vip")]
        public bool IsVip { get; set; }
    }
}
