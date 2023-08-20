using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Account Create Contract Json
    /// </summary>
    public class TronNetAccountCreateContractJson : TronNetContractBaseValueJson
    {
        /// <summary>
        /// owner_address
        /// </summary>
        [JsonProperty("account_address")]
        public virtual string AccountAddress { get; set; }
    }
}
