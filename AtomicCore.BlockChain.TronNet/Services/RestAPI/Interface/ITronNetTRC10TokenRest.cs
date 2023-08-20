using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TRC10 Token
    /// </summary>
    public interface ITronNetTRC10TokenRest
    {
        /// <summary>
        /// Get AssetIssue By Account
        /// </summary>
        /// <param name="tronAddress">tron address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAssetCollectionJson GetAssetIssueByAccount(string tronAddress, bool visible = true);

        /// <summary>
        /// Get AssetIssue By Id
        /// </summary>
        /// <param name="assertID">asset id</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAssetInfoJson GetAssetIssueById(int assertID, bool visible = true);

        /// <summary>
        /// Get AssetIssue List
        /// </summary>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAssetCollectionJson GetAssetIssueList(bool visible = true);

        /// <summary>
        /// Get Paginated AssentIssue LIst
        /// </summary>
        /// <param name="offset">page offset</param>
        /// <param name="limit">page limit</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAssetCollectionJson GetPaginatedAssetIssueList(int offset, int limit, bool visible = true);

        /// <summary>
        /// Transfer TRC10 token.
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="toAddress">receiving address</param>
        /// <param name="assetName">Token id</param>
        /// <param name="amount">amount</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson TransferAsset(string ownerAddress, string toAddress, string assetName, ulong amount, int? permissionID = null, bool visible = true);

        /// <summary>
        /// Create AssetIssue
        /// </summary>
        /// <param name="ownerAddress">Owner Address</param>
        /// <param name="tokenName">Token Name</param>
        /// <param name="tokenPrecision">Token Precision</param>
        /// <param name="tokenAbbr">Token Abbr</param>
        /// <param name="totalSupply">Token Total Supply</param>
        /// <param name="trxNum">
        /// Define the price by the ratio of trx_num/num(The unit of 'trx_num' is SUN)
        /// </param>
        /// <param name="num">
        /// Define the price by the ratio of trx_num/num
        /// </param>
        /// <param name="startTime">ICO StartTime</param>
        /// <param name="endTime">ICO EndTime</param>
        /// <param name="tokenDescription">Token Description</param>
        /// <param name="tokenUrl">Token Official Website Url</param>
        /// <param name="freeAssetNetLimit">Token Free Asset Net Limit</param>
        /// <param name="publicFreeAssetNetLimit">Token Public Free Asset Net Limit</param>
        /// <param name="frozenSupply">Token Frozen Supply</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson CreateAssetIssue(string ownerAddress, string tokenName, int tokenPrecision, string tokenAbbr, ulong totalSupply, ulong trxNum, ulong num, DateTime startTime, DateTime endTime, string tokenDescription, string tokenUrl, ulong freeAssetNetLimit, ulong publicFreeAssetNetLimit, TronNetFrozenSupplyJson frozenSupply, bool visible = true);

        /// <summary>
        /// Participate sAssetIssue
        /// </summary>
        /// <param name="toAddress">to address</param>
        /// <param name="ownerAddress">owner address</param>
        /// <param name="amount">amount</param>
        /// <param name="assetName">asset name</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson ParticipateAssetIssue(string toAddress, string ownerAddress, ulong amount, string assetName, bool visible = true);

        /// <summary>
        /// Unstake a token that has passed the minimum freeze duration.
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <param name="permissionID"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson UnfreezeAsset(string ownerAddress, int? permissionID = null, bool visible = true);

        /// <summary>
        /// Update basic TRC10 token information.
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <param name="description"></param>
        /// <param name="tokenUrl"></param>
        /// <param name="newLimit"></param>
        /// <param name="newPublicLimit"></param>
        /// <param name="permissionID"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson UpdateAsset(string ownerAddress, string tokenDescription
            , string tokenUrl, int newLimit, int newPublicLimit, int? permissionID, bool visible = true);

        /// <summary>
        /// Easy TRC10 token transfer. Create a TRC10 transfer transaction and broadcast directly.
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetCreateTransactionRestJson EasyTransferAsset(string passPhrase, string toAddress, string assetId, ulong amount, bool visible = true);

        /// <summary>
        /// TRC10 token easy transfer. Broadcast the created transaction directly.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        TronNetCreateTransactionRestJson EasyTransferAssetByPrivate(string privateKey, string toAddress, string assetId, ulong amount, bool visible = true);
    }
}
