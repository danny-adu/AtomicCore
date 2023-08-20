using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Asset Trc10 By Name Query
    /// </summary>
    public class TronGridAssetTrc10ByNameQuery : TronGridAssetTrc10Query
    {
        #region Propertys

        /// <summary>
        /// only_confirmed
        /// </summary>
        public bool? OnlyConfirmed { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// BuildQuery
        /// </summary>
        /// <returns></returns>
        protected override List<string> BuildQuery()
        {
            var paramList = base.BuildQuery();

            if(null != OnlyConfirmed)
                if(OnlyConfirmed.Value)
                    paramList.Add("only_confirmed=true");
                else
                    paramList.Add("only_confirmed=false");

            return paramList;
        }

        #endregion
    }
}
