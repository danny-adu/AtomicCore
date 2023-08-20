using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Asset Trc10 By Identifier Query
    /// </summary>
    public class TronGridAssetTrc10ByIdentifierQuery : ITronGridQuery
    {
        #region Propertys

        /// <summary>
        /// only_confirmed
        /// </summary>
        public bool? OnlyConfirmed { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// build query
        /// </summary>
        /// <returns></returns>
        protected List<string> BuildQuery()
        {
            var paramList = new List<string>();

            if (null != OnlyConfirmed)
                if (OnlyConfirmed.Value)
                    paramList.Add("only_confirmed=true");
                else
                    paramList.Add("only_confirmed=false");

            return paramList;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// get query parameters
        /// </summary>
        /// <returns></returns>
        public string GetQuery()
        {
            return string.Join("&", BuildQuery());
        }

        #endregion
    }
}
