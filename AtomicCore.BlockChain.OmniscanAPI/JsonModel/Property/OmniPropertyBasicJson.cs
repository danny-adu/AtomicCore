using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni asset details json
    /// </summary>
    public class OmniPropertyBasicJson
    {
        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// divisible
        /// </summary>
        [JsonProperty("divisible")]
        public bool Divisible { get; set; }

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
        /// propertyid
        /// </summary>
        [JsonProperty("propertyid")]
        public ulong PropertyId { get; set; }

        /// <summary>
        /// referenceaddress
        /// </summary>
        [JsonProperty("referenceaddress")]
        public string ReferenceAddress { get; set; }

        /// <summary>
        /// sendingaddress
        /// </summary>
        [JsonProperty("sendingaddress")]
        public string SendingAddress { get; set; }

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
        /// version
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
