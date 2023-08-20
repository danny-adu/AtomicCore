using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Contract Base Value Json
    /// </summary>
    public abstract class TronNetContractBaseValueJson
    {
        #region Propertys

        /// <summary>
        /// owner_address
        /// </summary>
        [JsonProperty("owner_address")]
        public virtual string OwnerAddress { get; set; }

        #endregion
    }
}
