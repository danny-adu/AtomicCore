using Newtonsoft.Json;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// omni tx info response
    /// </summary>
    public class OmniTxInfoResponse
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
        /// propertyname
        /// </summary>
        [JsonProperty("propertyname")]
        public string PropertyName { get; set; }

        /// <summary>
        /// reference address
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
        /// valid
        /// </summary>
        [JsonProperty("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// invalidreason
        /// </summary>
        [JsonProperty("invalidreason")]
        public string InvalidReason { get; set; }

        /// <summary>
        /// version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
