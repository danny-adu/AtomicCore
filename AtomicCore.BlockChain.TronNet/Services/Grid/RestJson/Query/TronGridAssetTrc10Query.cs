using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Asset Trc10 Query
    /// </summary>
    public class TronGridAssetTrc10Query : ITronGridQuery
    {
        #region Propertys

        /// <summary>
        /// total_supply,asc | total_supply,desc | start_time,asc | start_time,desc | end_time,asc | end_time,desc | id,asc | id,desc
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// null:null | limit default is 20 
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// last page fingerprint
        /// </summary>
        public string FingerPrint { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// build query
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> BuildQuery()
        {
            var paramList = new List<string>();

            if (null != Limit)
                paramList.Add($"limit={Limit.Value}");
            if (!string.IsNullOrEmpty(OrderBy))
                paramList.Add($"order_by={OrderBy}");
            if (!string.IsNullOrEmpty(FingerPrint))
                paramList.Add($"fingerprint={FingerPrint}");

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
