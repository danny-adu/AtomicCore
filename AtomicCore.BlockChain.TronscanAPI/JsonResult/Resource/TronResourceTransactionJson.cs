using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Resource Transaction Json
    /// </summary>
    public class TronResourceTransactionJson
    {
        /// <summary>
        /// Transaction Hash
        /// </summary>
        [JsonProperty("hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// owner address
        /// </summary>
        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// receiver address
        /// </summary>
        [JsonProperty("receiverAddress")]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// resource
        /// </summary>
        [JsonProperty("resource")]
        public TronResourceType Resource { get; set; }

        /// <summary>
        /// frozen balance
        /// </summary>
        [JsonProperty("frozenBalance")]
        public ulong FrozenBalance { get; set; }

        /// <summary>
        /// expire time
        /// </summary>
        [JsonProperty("expireTime")]
        public ulong ExpireTime { get; set; }

        /// <summary>
        /// resource value
        /// </summary>
        [JsonProperty("ResourceValue")]
        public decimal ResourceValue { get; set; }
    }
}
