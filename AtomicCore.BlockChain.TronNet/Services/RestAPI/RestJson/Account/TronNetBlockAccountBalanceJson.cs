using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Block Account Balance Json
    /// </summary>
    public class TronNetBlockAccountBalanceJson : TronNetValidRestJson
    {
        /// <summary>
        /// trx balance
        /// </summary>
        [JsonProperty("balance"), JsonConverter(typeof(TronNetTrxUnitJsonConverter))]
        public decimal TrxBalance { get; set; }

        /// <summary>
        /// block identifier
        /// </summary>
        [JsonProperty("block_identifier")]
        public TronNetBlockIdentifierJson BlockIdentifier { get; set; }
    }
}
