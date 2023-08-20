using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid MetaInfo
    /// </summary>
    public class TronGridMetaInfo
    {
        /// <summary>
        /// at # request timestamp millsecond timestamp
        /// </summary>
        [JsonProperty("at")]
        public ulong At { get; set; }

        /// <summary>
        /// fingerprint
        /// </summary>
        [JsonProperty("fingerprint")]
        public string FingerPrint { get; set; }

        /// <summary>
        /// links
        /// </summary>
        [JsonProperty("links"), JsonConverter(typeof(TronGridMetaLinkJsonConverter))]
        public TronGridMetaLinkInfo Links { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
    }
}
