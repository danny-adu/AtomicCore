namespace AtomicCore.BlockChain.BlockCypherAPI
{
    /// <summary>
    /// BlockCypherAPI Interface
    /// </summary>
    public interface IBlockCypherAPI
    {
        #region Address API

        /// <summary>
        /// General information about a blockchain is available by GET-ing the base resource.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#blockchain-api
        /// </summary>
        /// <param name="network"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        ChainEndpointResponse ChainEndpoint(BlockCypherNetwork network, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        /// <summary>
        /// If you want more data on a particular block, you can use the Block Hash endpoint.
        /// https://www.blockcypher.com/dev/bitcoin/?shell#block-hash-endpoint
        /// </summary>
        /// <param name="network"></param>
        /// <param name="blockHash"></param>
        /// <param name="cacheMode"></param>
        /// <param name="cacheSeconds"></param>
        /// <returns></returns>
        BlockCypherBlockResponse BlockHashEndpoint(BlockCypherNetwork network, string blockHash, BlockCypherCacheMode cacheMode = BlockCypherCacheMode.AbsoluteExpired, int cacheSeconds = 10);

        #endregion
    }
}
