namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// cache expiration mode
    /// </summary>
    public enum BscscanCacheMode
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
