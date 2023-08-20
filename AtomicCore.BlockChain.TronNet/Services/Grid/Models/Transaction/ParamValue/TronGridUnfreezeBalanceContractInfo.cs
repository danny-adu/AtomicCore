using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Unfreeze Balance Contract Info
    /// </summary>
    [TronNetParamValue(TronNetContractType.UnfreezeBalanceContract)]
    public class TronGridUnfreezeBalanceContractInfo : TronGridTransactionParamValue
    {
        #region Propertys

        /// <summary>
        /// resource
        /// </summary>
        [JsonProperty("resource")]
        public int Resource { get; set; }

        /// <summary>
        /// resource_type
        /// </summary>
        [JsonProperty("resource_type")]
        public string ResourceType { get; set; }

        /// <summary>
        /// resource_value
        /// </summary>
        [JsonProperty("resource_value")]
        public long ResourceValue { get; set; }

        /// <summary>
        /// receiver_address
        /// </summary>
        [JsonProperty("receiver_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string ReceiverAddress { get; set; }

        #endregion
    }
}
