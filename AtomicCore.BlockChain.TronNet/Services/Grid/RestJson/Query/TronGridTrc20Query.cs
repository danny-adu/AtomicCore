using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Trc20 Query
    /// </summary>
    public class TronGridTrc20Query : TronGridTransactionQuery
    {
        #region Propertys

        /// <summary>
        /// contract_address
        /// </summary>
        [JsonProperty("contract_address")]
        public string ContractAddress { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// build query
        /// </summary>
        /// <returns></returns>
        protected override List<string> BuildQuery()
        {
            var paramList = base.BuildQuery();

            if (!string.IsNullOrEmpty(ContractAddress))
                paramList.Add($"contract_address={ContractAddress}");

            return paramList;
        }

        #endregion
    }
}
