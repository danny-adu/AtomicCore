using Newtonsoft.Json;
using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// Bsc Internal Transaction Json
    /// </summary>
    public class BscInternalTransactionJson
    {
        /// <summary>
        /// isError
        /// </summary>
        [JsonProperty("isError")]
        public int IsError { get; set; }

        /// <summary>
        /// hash
        /// </summary>
        [JsonProperty("hash")]
        public string TxHash { get; set; }

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
        /// type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// traceId
        /// </summary>
        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        /// <summary>
        /// errCode
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        /// <summary>
        /// blockNumber
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }
    }
}
