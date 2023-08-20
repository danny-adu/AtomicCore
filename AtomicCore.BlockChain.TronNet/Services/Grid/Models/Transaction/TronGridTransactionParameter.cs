using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGridTransactionParameter
    /// </summary>
    public class TronGridTransactionParameter
    {
        /// <summary>
        /// type_url
        /// </summary>
        [JsonProperty("type_url")]
        public string TypeUrl { get; set; }

        /// <summary>
        /// value # parse the data with TronGridTransactionParameterValueHelper
        /// </summary>
        [JsonProperty("value"), JsonConverter(typeof(TronGridParamValueJsonConverter))]
        public ITronGridTransactionParamValue Value { get; set; }
    }
}
