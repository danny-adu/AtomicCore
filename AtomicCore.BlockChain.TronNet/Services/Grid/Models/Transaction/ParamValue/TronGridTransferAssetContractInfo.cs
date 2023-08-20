using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transfer Asset Contract Info
    /// </summary>
    [TronNetParamValue(TronNetContractType.TransferAssetContract)]
    public class TronGridTransferAssetContractInfo : TronGridTransactionParamValue
    {
        #region Propertys

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("asset_name")]
        public string AssetName { get; set; }

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("to_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string ToAddress { get; set; }

        /// <summary>
        /// amount
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Amount
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public decimal GetAmount(int decimals)
        {
            if (decimals < decimal.Zero)
                return decimal.Zero;

            if (decimals > 1)
                return TronNetUntils.ValueToAmount(Amount, decimals);
            else
                return Amount;
        }

        #endregion
    }
}
