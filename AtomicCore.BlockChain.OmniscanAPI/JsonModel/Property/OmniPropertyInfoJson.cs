using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni property info json
    /// </summary>
    public class OmniPropertyInfoJson
    {
        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// block
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
        /// category
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// creationtxid
        /// </summary>
        [JsonProperty("creationtxid")]
        public string CreationTxid { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

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
        /// fixedissuance
        /// </summary>
        [JsonProperty("fixedissuance")]
        public bool FixedIssuance { get; set; }

        /// <summary>
        /// flags
        /// </summary>
        [JsonProperty("flags")]
        public JObject Flags { get; set; }

        /// <summary>
        /// freezingenabled
        /// </summary>
        [JsonProperty("freezingenabled")]
        public bool FreezingEnabled { get; set; }

        /// <summary>
        /// ismine
        /// </summary>
        [JsonProperty("ismine")]
        public bool Ismine { get; set; }

        /// <summary>
        /// issuer
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// managedissuance
        /// </summary>
        [JsonProperty("managedissuance")]
        public bool ManageDissuance { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// positioninblock
        /// </summary>
        [JsonProperty("positioninblock")]
        public int PositionInblock { get; set; }

        /// <summary>
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// propertyname
        /// </summary>
        [JsonProperty("propertyname")]
        public string PropertyName { get; set; }

        /// <summary>
        /// propertytype
        /// </summary>
        [JsonProperty("propertytype")]
        public string PropertyType { get; set; }

        /// <summary>
        /// rdata
        /// </summary>
        [JsonProperty("rdata")]
        public string RData { get; set; }

        /// <summary>
        /// registered
        /// </summary>
        [JsonProperty("registered")]
        public bool Registered { get; set; }

        /// <summary>
        /// sendingaddress
        /// </summary>
        [JsonProperty("sendingaddress")]
        public string SendingAddress { get; set; }

        /// <summary>
        /// subcategory
        /// </summary>
        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        /// <summary>
        /// totaltokens
        /// </summary>
        [JsonProperty("totaltokens")]
        public decimal TotalTokens { get; set; }

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
        public int Version { get; set; }
    }
}
