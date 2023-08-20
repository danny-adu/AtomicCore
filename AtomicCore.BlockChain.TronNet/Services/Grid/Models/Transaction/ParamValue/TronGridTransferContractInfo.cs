using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid TransferContract Info
    /// </summary>
    [TronNetParamValue(TronNetContractType.TransferContract)]
    public class TronGridTransferContractInfo : TronGridTransactionParamValue
    {
        #region Propertys

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("to_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string ToAddress { get; set; }

        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount"),JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal Amount { get; set; }

        #endregion
    }
}
