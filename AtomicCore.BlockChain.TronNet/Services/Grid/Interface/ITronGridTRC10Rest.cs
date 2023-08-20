namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid TRC10 Rest
    /// https://cn.developers.tron.network/reference/list-all-assets-trc10-tokens-on-chain
    /// </summary>
    public interface ITronGridTRC10Rest
    {
        /// <summary>
        /// Get a list of all TRC10s
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        TronGridRestResult<TronGridAssetTrc10Info> GetTrc10List(TronGridAssetTrc10Query query = null);

        /// <summary>
        /// Query TRC10 by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        TronGridRestResult<TronGridAssetTrc10Info> GetTrc10ListByName(string name, TronGridAssetTrc10ByNameQuery query = null);

        /// <summary>
        /// Query TRC10 by ID or issuer
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        TronGridRestResult<TronGridAssetTrc10Info> GetTrc10ListByIdentifier(string identifier, TronGridAssetTrc10ByIdentifierQuery query = null);
    }
}
