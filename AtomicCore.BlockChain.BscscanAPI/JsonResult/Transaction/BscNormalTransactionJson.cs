using Newtonsoft.Json;
using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// Bsc Normal Transaction Json
    /// </summary>
    public class BscNormalTransactionJson
    {
        /// <summary>
        /// isError
        /// </summary>
        [JsonProperty("isError")]
        public int IsError { get; set; }

        /// <summary>
        /// txreceipt_status
        /// </summary>
        [JsonProperty("txreceipt_status")]
        public int TxReceiptStatus { get; set; }

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
        /// timeStamp
        /// </summary>
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// from
        /// </summary>
        [JsonProperty("from")]
        public string TxFrom { get; set; }

        /// <summary>
        /// to
        /// </summary>
        [JsonProperty("to")]
        public string TxTo { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [JsonProperty("value"), JsonConverter(typeof(BscBNBConverter))]
        public decimal TxValue { get; set; }

        /// <summary>
        /// contractAddress
        /// </summary>
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// input
        /// </summary>
        [JsonProperty("input")]
        public string TxInput { get; set; }

        /// <summary>
        /// gasPrice
        /// </summary>
        [JsonProperty("gasPrice")]
        public long TxGasPrice { get; set; }

        /// <summary>
        /// gas
        /// </summary>
        [JsonProperty("gas")]
        public long TxGas { get; set; }

        /// <summary>
        /// gasUsed
        /// </summary>
        [JsonProperty("gasUsed")]
        public long GasUsed { get; set; }

        /// <summary>
        /// transactionIndex
        /// </summary>
        [JsonProperty("transactionIndex")]
        public int TransactionIndex { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        /// <summary>
        /// cumulativeGasUsed
        /// </summary>
        [JsonProperty("cumulativeGasUsed")]
        public long CumulativeGasUsed { get; set; }

        /// <summary>
        /// blockHash
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// blockNumber
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }
    }
}
