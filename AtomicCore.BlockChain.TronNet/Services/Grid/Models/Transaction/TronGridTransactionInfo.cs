using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transaction Info
    /// </summary>
    public class TronGridTransactionInfo
    {
        /// <summary>
        /// txID
        /// </summary>
        [JsonProperty("txID")]
        public string TxHash { get; set; }

        /// <summary>
        /// net_usage
        /// </summary>
        [JsonProperty("net_usage")]
        public int NetUsage { get; set; }

        /// <summary>
        /// net_fee
        /// </summary>
        [JsonProperty("net_fee")]
        public int NetFee { get; set; }

        /// <summary>
        /// energy_usage
        /// </summary>
        [JsonProperty("energy_usage")]
        public int EnergyUsage { get; set; }

        /// <summary>
        /// energy_fee
        /// </summary>
        [JsonProperty("energy_fee")]
        public int EnergyFee { get; set; }

        /// <summary>
        /// energy_usage_total
        /// </summary>
        [JsonProperty("energy_usage_total")]
        public int EnergyUsageTotal { get; set; }

        /// <summary>
        /// signature
        /// </summary>
        [JsonProperty("signature")]
        public string[] Signature { get; set; }

        /// <summary>
        /// raw_data_hex
        /// </summary>
        [JsonProperty("raw_data_hex")]
        public string RawDataHex { get; set; }

        /// <summary>
        /// raw_data
        /// </summary>
        [JsonProperty("raw_data")]
        public TronGridTransactionRaw RawData { get; set; }

        /// <summary>
        /// ret
        /// </summary>
        [JsonProperty("ret")]
        public TronGridTransactionRet[] Ret { get; set; }

        /// <summary>
        /// blockNumber
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }

        /// <summary>
        /// block_timestamp
        /// </summary>
        [JsonProperty("block_timestamp")]
        public long BlockTimestamp { get; set; }

        /// <summary>
        /// unfreeze_amount # TRX
        /// </summary>
        [JsonProperty("unfreeze_amount"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal UnfreezeAmount { get; set; }

        //internal_transactions
    }
}
