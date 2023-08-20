using Newtonsoft.Json;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Contract MetaData Json
    /// </summary>
    public class TronNetContractMetaDataJson : TronNetValidRestJson
    {
        /// <summary>
        /// code_hash
        /// </summary>
        [JsonProperty("code_hash")]
        public string CodeHash { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// origin_address
        /// </summary>
        [JsonProperty("origin_address")]
        public string OriginAddress { get; set; }

        /// <summary>
        /// origin_energy_limit
        /// </summary>
        [JsonProperty("origin_energy_limit")]
        public int OriginEnergyLimit { get; set; }

        /// <summary>
        /// consume_user_resource_percent
        /// </summary>
        [JsonProperty("consume_user_resource_percent")]
        public int ConsumeUserResourcePercent { get; set; }

        /// <summary>
        /// bytecode
        /// </summary>
        [JsonProperty("bytecode")]
        public string Bytecode { get; set; }

        /// <summary>
        /// abi
        /// </summary>
        [JsonProperty("abi")]
        public TronNetContractFuncMetaDataJson Abi { get; set; }
    }
}
