using Newtonsoft.Json;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc account balance json
    /// </summary>
    public class BscAccountBalanceJson
    {
        /// <summary>
        /// account
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// balance
        /// </summary>
        [JsonProperty("balance"),JsonConverter(typeof(BscBNBConverter))]
        public decimal Balance { get; set; }
    }
}
