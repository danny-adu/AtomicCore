using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// tron databse overview
    /// </summary>
    public class TronDatabaseOverviewJson
    {
        /// <summary>
        /// Block Height
        /// </summary>
        [JsonProperty("block"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// Confirmed Block
        /// </summary>
        [JsonProperty("confirmedBlock"), JsonConverter(typeof(BizTronULongJsonConverter))]
        public ulong ConfirmedBlock { get; set; }
    }
}
