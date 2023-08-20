using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transaction Ret
    /// </summary>
    public class TronGridTransactionRet
    {
        /// <summary>
        /// contractRet
        /// </summary>
        [JsonProperty("contractRet")]
        public string ContractRet { get; set; }

        /// <summary>
        /// fee
        /// </summary>
        [JsonProperty("fee")]
        public int Fee { get; set; }
    }
}
