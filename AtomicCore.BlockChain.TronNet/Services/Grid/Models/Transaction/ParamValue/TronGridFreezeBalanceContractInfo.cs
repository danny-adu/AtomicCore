using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Freeze Balance Contract Info
    /// </summary>
    [TronNetParamValue(TronNetContractType.FreezeBalanceContract)]
    public class TronGridFreezeBalanceContractInfo : TronGridTransactionParamValue
    {
        #region Propertys

        /// <summary>
        /// resource
        /// </summary>
        [JsonProperty("resource")]
        public int Resource { get; set; }

        /// <summary>
        /// frozen_duration
        /// </summary>
        [JsonProperty("frozen_duration")]
        public int FrozenDuration { get; set; }

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
        /// frozen_balance # TRX
        /// </summary>
        [JsonProperty("frozen_balance"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal FrozenBalance { get; set; }

        /// <summary>
        /// receiver_address
        /// </summary>
        [JsonProperty("receiver_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string ReceiverAddress { get; set; }

        #endregion
    }
}
