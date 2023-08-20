using Newtonsoft.Json;
using System.Numerics;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Address Asset Balance Json
    /// </summary>
    public class TronAssetBalanceJson : TronTokenAssetJson
    {
        /// <summary>
        /// account balance
        /// </summary>
        [JsonProperty("balance")]
        public BigInteger Balance { get; set; }

        /// <summary>
        /// converted trx value amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
