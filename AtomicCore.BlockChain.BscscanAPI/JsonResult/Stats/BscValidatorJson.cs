using Newtonsoft.Json;
using System.Numerics;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// bsc validator json
    /// </summary>
    public class BscValidatorJson
    {
        /// <summary>
        /// validatorName
        /// </summary>
        [JsonProperty("validatorName")]
        public string ValidatorName { get; set; }

        /// <summary>
        /// validatorAddress
        /// </summary>
        [JsonProperty("validatorAddress")]
        public string ValidatorAddress { get; set; }

        /// <summary>
        /// validatorStatus
        /// </summary>
        [JsonProperty("validatorStatus")]
        public int ValidatorStatus { get; set; }

        /// <summary>
        /// validatorVotingPower
        /// </summary>
        [JsonProperty("validatorVotingPower")]
        public BigInteger ValidatorVotingPower { get; set; }

        /// <summary>
        /// validatorVotingPowerProportion
        /// </summary>
        [JsonProperty("validatorVotingPowerProportion")]
        public decimal ValidatorVotingPowerProportion { get; set; }
    }
}
