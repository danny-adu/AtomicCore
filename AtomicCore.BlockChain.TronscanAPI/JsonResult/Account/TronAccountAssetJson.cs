using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tron Token Asset Json
    /// </summary>
    public class TronAccountAssetJson
    {
        /// <summary>
        /// trc20 token balances
        /// </summary>
        [JsonProperty("trc20token_balances")]
        public TronAssetBalanceJson[] Trc20Balances { get; set; }

        /// <summary>
        /// transactions_out
        /// </summary>
        [JsonProperty("transactions_out")]
        public int TransactionsOut { get; set; }

        /// <summary>
        /// acquiredDelegateFrozenForBandWidth
        /// </summary>
        [JsonProperty("acquiredDelegateFrozenForBandWidth")]
        public ulong AcquiredDelegateFrozenForBandWidth { get; set; }

        /// <summary>
        /// rewardNum
        /// </summary>
        [JsonProperty("rewardNum")]
        public int RewardNum { get; set; }

        /// <summary>
        /// ownerPermission
        /// </summary>
        [JsonProperty("ownerPermission")]
        public TronAccountPermissionJson OwnerPermission { get; set; }

        /// <summary>
        /// tokenBalances
        /// </summary>
        [JsonProperty("tokenBalances")]
        public TronAccountTokenBalanceJson[] TokenBalances { get; set; }

        /// <summary>
        /// delegateFrozenForEnergy
        /// </summary>
        [JsonProperty("delegateFrozenForEnergy")]
        public ulong DelegateFrozenForEnergy { get; set; }

        /// <summary>
        /// balances
        /// </summary>
        [JsonProperty("balances")]
        public TronAccountTokenBalanceJson[] Balances { get; set; }

        /// <summary>
        /// trc721token_balances
        /// </summary>
        [JsonProperty("trc721token_balances")]
        public JArray TRC721TokenBalances { get; set; }

        /// <summary>
        /// TRX Balance
        /// </summary>
        [JsonProperty("balance")]
        public BigInteger Balance { get; set; }

        /// <summary>
        /// voteTotal
        /// </summary>
        [JsonProperty("voteTotal")]
        public ulong VoteTotal { get; set; }

        /// <summary>
        /// totalFrozen
        /// </summary>
        [JsonProperty("totalFrozen")]
        public BigInteger TotalFrozen { get; set; }

        /// <summary>
        /// tokens
        /// </summary>
        [JsonProperty("tokens")]
        public TronAccountTokenBalanceJson[] Tokens { get; set; }

        /// <summary>
        /// delegated
        /// </summary>
        [JsonProperty("delegated")]
        public JObject Delegated { get; set; }

        /// <summary>
        /// transactions_in
        /// </summary>
        [JsonProperty("transactions_in")]
        public ulong TransactionsIn { get; set; }

        /// <summary>
        /// totalTransactionCount
        /// </summary>
        [JsonProperty("totalTransactionCount")]
        public ulong TotalTransactionCount { get; set; }

        /// <summary>
        /// representative
        /// </summary>
        [JsonProperty("representative")]
        public JObject Representative { get; set; }

        /// <summary>
        /// frozenForBandWidth
        /// </summary>
        [JsonProperty("frozenForBandWidth")]
        public BigInteger FrozenForBandWidth { get; set; }

        /// <summary>
        /// reward
        /// </summary>
        [JsonProperty("reward")]
        public ulong Reward { get; set; }

        /// <summary>
        /// addressTagLogo
        /// </summary>
        [JsonProperty("addressTagLogo")]
        public string AddressTagLogo { get; set; }

        /// <summary>
        /// allowExchange
        /// </summary>
        [JsonProperty("allowExchange")]
        public JArray AllowExchange { get; set; }

        /// <summary>
        /// address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// frozen_supply
        /// </summary>
        [JsonProperty("frozen_supply")]
        public JArray FrozenSupply { get; set; }

        /// <summary>
        /// bandwidth
        /// </summary>
        [JsonProperty("bandwidth")]
        public TronAccountBandwidthJson Bandwidth { get; set; }

        /// <summary>
        /// date_created
        /// </summary>
        [JsonProperty("date_created")]
        public ulong DateCreated { get; set; }

        /// <summary>
        /// accountType
        /// </summary>
        [JsonProperty("accountType")]
        public int AccountType { get; set; }

        /// <summary>
        /// exchanges
        /// </summary>
        [JsonProperty("exchanges")]
        public JArray Exchanges { get; set; }

        /// <summary>
        /// frozen
        /// </summary>
        [JsonProperty("frozen")]
        public JObject Frozen { get; set; }

        /// <summary>
        /// accountResource
        /// </summary>
        [JsonProperty("accountResource")]
        public JObject AccountResource { get; set; }

        /// <summary>
        /// transactions
        /// </summary>
        [JsonProperty("transactions")]
        public ulong Transactions { get; set; }

        /// <summary>
        /// witness
        /// </summary>
        [JsonProperty("witness")]
        public ulong Witness { get; set; }

        /// <summary>
        /// delegateFrozenForBandWidth
        /// </summary>
        [JsonProperty("delegateFrozenForBandWidth")]
        public BigInteger DelegateFrozenForBandWidth { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// frozenForEnergy
        /// </summary>
        [JsonProperty("frozenForEnergy")]
        public ulong FrozenForEnergy { get; set; }

        /// <summary>
        /// acquiredDelegateFrozenForEnergy
        /// </summary>
        [JsonProperty("acquiredDelegateFrozenForEnergy")]
        public ulong AcquiredDelegateFrozenForEnergy { get; set; }

        /// <summary>
        /// activePermissions
        /// </summary>
        [JsonProperty("activePermissions")]
        public TronAccountOperatePermissionJson[] ActivePermissions { get; set; }
    }
}
