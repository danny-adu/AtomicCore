using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Delegated Resource Account
    /// </summary>
    public class TronNetDelegatedResourceAccountJson : TronNetValidRestJson
    {
        /// <summary>
        /// account
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// toAccounts
        /// </summary>
        [JsonProperty("toAccounts")]
        public string[] ToAccounts { get; set; }
    }
}
