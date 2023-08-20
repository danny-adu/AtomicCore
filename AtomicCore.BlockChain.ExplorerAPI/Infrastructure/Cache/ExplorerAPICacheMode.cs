namespace AtomicCore.BlockChain.ExplorerAPI
{
    /// <summary>
    /// cache expiration mode
    /// </summary>
    public enum ExplorerAPICacheMode
    {
        /// <summary>
        /// none
        /// </summary>
        None = 0,

        /// <summary>
        /// absolute expired
        /// </summary>
        AbsoluteExpired = 1,

        /// <summary>
        /// slide expired
        /// </summary>
        SlideExpired = 2
    }
}
