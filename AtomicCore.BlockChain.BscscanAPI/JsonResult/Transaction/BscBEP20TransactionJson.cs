using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc bep-20 transaction json
    /// </summary>
    public class BscBEP20TransactionJson
    {
        #region Variables

        /// <summary>
        /// transfer method id
        /// </summary>
        public const string TRANSFER_METHOD_ID = "0xa9059cbb";

        #endregion

        #region Propertys

        /// <summary>
        /// blockNumber
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }

        /// <summary>
        /// timeStamp
        /// </summary>
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// hash
        /// </summary>
        [JsonProperty("hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// nonce
        /// </summary>
        [JsonProperty("nonce")]
        public int TxNonce { get; set; }

        /// <summary>
        /// blockHash
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// from
        /// </summary>
        [JsonProperty("from")]
        public string TxFrom { get; set; }

        /// <summary>
        /// contractAddress
        /// </summary>
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// to
        /// </summary>
        [JsonProperty("to")]
        public string TxTo { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value")]
        public decimal TxValue { get; set; }

        /// <summary>
        /// tokenName
        /// </summary>
        [JsonProperty("tokenName")]
        public string TokenName { get; set; }

        /// <summary>
        /// tokenSymbol
        /// </summary>
        [JsonProperty("tokenSymbol")]
        public string TokenSymbol { get; set; }

        /// <summary>
        /// tokenDecimal
        /// </summary>
        [JsonProperty("tokenDecimal")]
        public int TokenDecimal { get; set; }

        /// <summary>
        /// transactionIndex
        /// </summary>
        [JsonProperty("transactionIndex")]
        public int TransactionIndex { get; set; }

        /// <summary>
        /// gas
        /// </summary>
        [JsonProperty("gas")]
        public long TxGas { get; set; }

        /// <summary>
        /// gasPrice
        /// </summary>
        [JsonProperty("gasPrice")]
        public long TxGasPrice { get; set; }

        /// <summary>
        /// gasUsed
        /// </summary>
        [JsonProperty("gasUsed")]
        public long GasUsed { get; set; }

        /// <summary>
        /// cumulativeGasUsed
        /// </summary>
        [JsonProperty("cumulativeGasUsed")]
        public long CumulativeGasUsed { get; set; }

        /// <summary>
        /// input
        /// </summary>
        [JsonProperty("input")]
        public string TxInput { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// get toAddress
        /// </summary>
        /// <returns></returns>
        public string GetToAddress()
        {
            if (string.IsNullOrEmpty(TxInput))
                return string.Empty;
            if (TxInput.Length != 138)
                return string.Empty;
            if (!TxInput.StartsWith(TRANSFER_METHOD_ID, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            return $"0x{TxInput.Substring(34, 40)}";
        }

        /// <summary>
        /// get bep-20 transfer amount
        /// </summary>
        /// <returns></returns>
        public decimal GetTransferAmount()
        {
            if (string.IsNullOrEmpty(TxInput))
                return decimal.Zero;
            if (TxInput.Length != 138)
                return decimal.Zero;
            if (!TxInput.StartsWith(TRANSFER_METHOD_ID, StringComparison.OrdinalIgnoreCase))
                return decimal.Zero;

            string hex = TxInput.Substring(74, 64);
            if (string.IsNullOrEmpty(hex))
                return decimal.Zero;

            hex = Regex.Replace(hex, @"^0+", string.Empty, RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(hex))
                return decimal.Zero;

            if (TokenDecimal > 0)
                return Nethereum.Util.UnitConversion.Convert.FromWei(new Nethereum.Hex.HexTypes.HexBigInteger(hex), this.TokenDecimal);
            else
                return (decimal)new Nethereum.Hex.HexTypes.HexBigInteger(hex).Value;
        }

        #endregion
    }
}
