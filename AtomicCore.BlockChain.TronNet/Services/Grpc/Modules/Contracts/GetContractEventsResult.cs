using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Contract Event Result
    /// </summary>
    public class GetContractEventsResult
    {
        /// <summary>
        /// Event Flag
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Event Data
        /// </summary>
        [JsonProperty("data")]
        public List<GetContractEventsResultItem> Data { get; set; }

        /// <summary>
        /// Event MetaData
        /// </summary>
        [JsonProperty("meta")]
        public GetContractEventsResultMetaData Meta { get; set; }
    }

    /// <summary>
    /// Contract Event Item Data
    /// </summary>
    public class GetContractEventsResultItem
    {
        /// <summary>
        /// Block Number
        /// </summary>
        [JsonProperty("block_number")]
        public long BlockNumber { get; set; }

        /// <summary>
        /// Block Timestamp
        /// </summary>
        [JsonProperty("block_timestamp")]
        public long BlockTimestamp { get; set; }

        /// <summary>
        /// Contract Address
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// Event Name
        /// </summary>
        [JsonProperty("event_name")]
        public string EventName { get; set; }

        /// <summary>
        /// Transaction Result
        /// </summary>
        [JsonProperty("result")]
        public GetContractEventsResultTransaction Result { get; set; }

        /// <summary>
        /// TXID
        /// </summary>
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }
    }

    /// <summary>
    /// Contract Event Transaction Info
    /// </summary>
    public class GetContractEventsResultTransaction
    {
        /// <summary>
        /// From Address
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// To Addresss
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Hex Value
        /// </summary>
        [JsonProperty("value")]
        public string Value { internal get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; internal set; }
    }

    /// <summary>
    /// Contract Event MetaData
    /// </summary>
    public class GetContractEventsResultMetaData
    {
        /// <summary>
        /// Fingerprint
        /// </summary>
        [JsonProperty("fingerprint")]
        public string Fingerprint { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
    }

}
