using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc bep-721 transaction json
    /// </summary>
    public class BscBEP721TransactionJson
    {
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
        /// tokenID
        /// </summary>
        [JsonProperty("tokenID")]
        public string TokenID { get; set; }

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
    }
}
