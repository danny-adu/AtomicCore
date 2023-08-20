using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Contract Function MetaData Json
    /// </summary>
    public class TronNetContractFuncMetaDataJson
    {
        /// <summary>
        /// entrys
        /// </summary>
        [JsonProperty("entrys")]
        public TronNetAbiFuncEntryJson[] Entrys { get; set; }
    }
}
