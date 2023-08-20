using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Base Query
    /// </summary>
    public class TronGridBaseQuery : ITronGridQuery
    {
        #region Propertys

        /// <summary>
        /// null:null | true:only_confirmed | false:only_unconfirmed
        /// </summary>
        public bool? OnlyConfirmed { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// build query
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> BuildQuery()
        {
            var paramList = new List<string>();

            if (null != this.OnlyConfirmed)
                if (this.OnlyConfirmed.Value)
                    paramList.Add("only_confirmed=true");
                else
                    paramList.Add("only_unconfirmed=true");

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
