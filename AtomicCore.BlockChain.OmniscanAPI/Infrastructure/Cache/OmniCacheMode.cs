namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// cache expiration mode
    /// </summary>
    public enum OmniCacheMode
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
