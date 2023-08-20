using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni history tx info json
    /// </summary>
    public class OmniPropertyHistoryTxJson
    {
        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// blockhash
        /// </summary>
        [JsonProperty("blockhash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// blocktime
        /// </summary>
        [JsonProperty("blocktime")]
        public ulong BlockTimestamp { get; set; }

        /// <summary>
        /// tx category
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// deadline
        /// </summary>
        [JsonProperty("deadline")]
        public ulong Deadline { get; set; }

        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

        /// <summary>
        /// earlybonus
        /// </summary>
        [JsonProperty("earlybonus")]
        public int Earlybonus { get; set; }

        /// <summary>
        /// ecosystem
        /// </summary>
        [JsonProperty("ecosystem")]
        public string Ecosystem { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        /// <summary>
        /// ismine
        /// </summary>
        [JsonProperty("ismine")]
        public bool Ismine { get; set; }

        /// <summary>
        /// percenttoissuer
        /// </summary>
        [JsonProperty("percenttoissuer")]
        public decimal Percenttoissuer { get; set; }

        /// <summary>
        /// positioninblock
        /// </summary>
        [JsonProperty("positioninblock")]
        public int PositionInblock { get; set; }

        /// <summary>
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong Propertyid { get; set; }

        /// <summary>
        /// propertyiddesired
        /// </summary>
        [JsonProperty("propertyiddesired")]
        public int PropertyIdDesired { get; set; }

        /// <summary>
        /// propertyname
        /// </summary>
        [JsonProperty("propertyname")]
        public string Propertyname { get; set; }

        /// <summary>
        /// propertytype
        /// </summary>
        [JsonProperty("propertytype")]
        public string Propertytype { get; set; }

        /// <summary>
        /// sendingaddress
        /// </summary>
        [JsonProperty("sendingaddress")]
        public string SendingAddress { get; set; }

        /// <summary>
        /// subcategory
        /// </summary>
        [JsonProperty("subcategory")]
        public string SubCategory { get; set; }

        /// <summary>
        /// tokensperunit
        /// </summary>
        [JsonProperty("tokensperunit")]
        public decimal TokenSperunit { get; set; }

        /// <summary>
        /// txid
        /// </summary>
        [JsonProperty("txid")]
        public string Txid { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// type_int
        /// </summary>
        [JsonProperty("type_int")]
        public int TypeInt { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// valid
        /// </summary>
        [JsonProperty("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
