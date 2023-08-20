using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.BscscanAPI
{
    /// <summary>
    /// IBscBlocks Interface
    /// </summary>
    public interface IBscBlocks
    {
        /// <summary>
        /// Returns the block reward awarded for validating a certain block. 
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscBlockRewardJson> GetBlockRewardByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the estimated time remaining, in seconds, until a certain block is validated.
        /// </summary>
        /// <param name="blockNo">the integer block number to check block rewards for eg. 12697906</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<BscBlockEstimatedJson> GetBlockEstimatedByNumber(long blockNo, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the block number that was validated at a certain timestamp.
        /// Tip : 
        ///         Convert a regular date-time to a Unix timestamp.
        /// </summary>
        /// <param name="timestamp">the integer representing the Unix timestamp in seconds.</param>
        /// <param name="closest">the closest available block to the provided timestamp, either before or after</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanSingleResult<long> GetBlockNumberByTimestamp(long timestamp, BscClosest closest, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);

        /// <summary>
        /// Returns the daily average block size within a date range.
        /// </summary>
        /// <param name="startdate">the starting date in yyyy-MM-dd format, eg. 2021-08-01</param>
        /// <param name="enddate">the ending date in yyyy-MM-dd format, eg. 2021-08-31</param>
        /// <param name="sort">the sorting preference, use asc to sort by ascending and desc to sort by descending</param>
        /// <param name="network">network</param>
        /// <param name="cacheMode">cache mode</param>
        /// <param name="expiredSeconds">expired seconds</param>
        /// <returns></returns>
        BscscanListResult<BscBlockAvgSizeJson> GetDailyAverageBlockSize(DateTime startdate, DateTime enddate, BscSort sort = BscSort.Desc, BscNetwork network = BscNetwork.BscMainnet, BscscanCacheMode cacheMode = BscscanCacheMode.None, int expiredSeconds = 10);
    }
}
