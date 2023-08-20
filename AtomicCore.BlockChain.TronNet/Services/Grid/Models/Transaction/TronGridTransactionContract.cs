using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transaction Contract
    /// </summary>
    public class TronGridTransactionContract
    {
        /// <summary>
        /// contract type
        /// </summary>
        [JsonProperty("type"),JsonConverter(typeof(TronNetContractTypeJsonConverter))]
        public TronNetContractType Type { get; set; }

        /// <summary>
        /// parameter
        /// </summary>
        [JsonProperty("parameter")]
        public TronGridTransactionParameter Parameter { get; set; }
    }
}
