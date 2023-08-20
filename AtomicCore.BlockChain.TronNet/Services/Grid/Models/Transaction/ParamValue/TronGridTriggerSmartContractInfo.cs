using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Trigger SmartContract Info
    /// </summary>
    [TronNetParamValue(TronNetContractType.TriggerSmartContract)]
    public class TronGridTriggerSmartContractInfo : TronGridTransactionParamValue
    {
        #region Variables

        /// <summary>
        /// transfer
        /// </summary>
        private const string c_transfer = "a9059cbb";

        #endregion

        #region Propertys

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("contract_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string ContractAddress { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// get toAddress
        /// </summary>
        /// <param name="isBase58Check"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public string GetToAddress(bool isBase58Check = true, TronNetwork network = TronNetwork.MainNet)
        {
            if (string.IsNullOrEmpty(Data))
                return string.Empty;
            if (Data.Length < 72)
                return string.Empty;
            if (!Data.StartsWith(c_transfer))
                return string.Empty;

            string hex_address = Data.Substring(30, 42);
            if (isBase58Check)
                return TronNetECKey.ConvertToTronAddressFromHexAddress(hex_address, network);
            else
                return hex_address;
        }

        /// <summary>
        /// Get Raw Amount
        /// </summary>
        /// <returns></returns>
        public System.Numerics.BigInteger GetRawAmount()
        {
            if (string.IsNullOrEmpty(Data))
                return System.Numerics.BigInteger.Zero;
            if (Data.Length < 136)
                return System.Numerics.BigInteger.Zero;
            if (!Data.StartsWith(c_transfer))
                return System.Numerics.BigInteger.Zero;

            string hex_value = Data.Substring(72, 64);
            hex_value = TronNetUntils.RemoveHexZero(hex_value, TronNetHexCuteZeroStrategy.Left, 0, true);

            return TronNetUntils.HexStrToBigInteger(hex_value);
        }

        /// <summary>
        /// Get Amount
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public decimal GetAmount(int decimals)
        {
            if (decimals < decimal.Zero)
                return decimal.Zero;

            var raw = GetRawAmount();
            if (decimals == 0)
                return (decimal)raw;
            else
                return TronNetUntils.ValueToAmount((long)raw, decimals);
        }

        #endregion
    }
}
