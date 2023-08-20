using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Transaction Info Json
    /// </summary>
    public class TronTransactionInfoJson
    {
        /// <summary>
        /// block height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// transaction id
        /// </summary>
        [JsonProperty("hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// transaction timestamp
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// owner address
        /// </summary>
        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        /// <summary>
        /// signature addresses
        /// </summary>
        [JsonProperty("signature_addresses")]
        public string[] SignatureAddresses { get; set; }

        /// <summary>
        /// ContractType
        /// </summary>
        [JsonProperty("contractType")]
        public TronscanContractType ContractType { get; set; }

        /// <summary>
        /// toAddress
        /// </summary>
        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// confirmations
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// confirmed
        /// </summary>
        [JsonProperty("confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// revert
        /// </summary>
        [JsonProperty("revert")]
        public bool Revert { get; set; }

        /// <summary>
        /// contract return state
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }

        /// <summary>
        /// contractData
        /// </summary>
        [JsonProperty("contractData")]
        public TronContractDataJson ContractData { get; set; }

        /// <summary>
        /// data
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// cost
        /// </summary>
        [JsonProperty("cost")]
        public TronTransactionCostJson Cost { get; set; }

        /// <summary>
        /// trigger info
        /// </summary>
        [JsonProperty("trigger_info")]
        public TronContractTriggerInfoJson TriggerInfo { get; set; }

        /////// <summary>
        /////// internal transactions
        /////// </summary>
        ////[JsonProperty("internal_transactions")]
        ////public object InternalTransactions { get; set; }

        /// <summary>
        /// FeeLimit
        /// </summary>
        [JsonProperty("fee_limit"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong FeeLimit { get; set; }

        /// <summary>
        /// srConfirmList
        /// </summary>
        [JsonProperty("srConfirmList")]
        public TronSRConfirmJson[] SrConfirmList { get; set; }

        /// <summary>
        /// contract type
        /// </summary>
        [JsonProperty("contract_type")]
        public string ContractTypeName { get; set; }

        /// <summary>
        /// EventCount
        /// </summary>
        [JsonProperty("event_count")]
        public int EventCount { get; set; }

        ///////// <summary>
        ///////// info
        ///////// </summary>
        //////[JsonProperty("info")]
        //////public object Info { get; set; }

        /// <summary>
        /// Contract Map
        /// </summary>
        [JsonProperty("contractMap")]
        public IReadOnlyDictionary<string, string> ContractMap { get; set; }
    }
}
