using System.Collections.Generic;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// Omni Address V2 Response
    /// </summary>
    public class OmniAddressV2Response : OmniErrorResponse
    {
        #region Propertys

        /// <summary>
        /// Address Balance
        /// </summary>
        public Dictionary<string, OmniAssetCollectionJson> AddressBalances { get; set; }

        #endregion
    }
}
