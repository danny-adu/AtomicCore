using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Update Contract Json
    /// </summary>
    public class TronNetAccountUpdateContractJson : TronNetContractBaseValueJson
    {
        /// <summary>
        /// Account Name Hex
        /// </summary>
        [JsonProperty("account_name")]
        public string AccountName { get; set; }
    }
}
