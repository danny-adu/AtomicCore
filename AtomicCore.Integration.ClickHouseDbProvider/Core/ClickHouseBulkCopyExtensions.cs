using ClickHouse.Client.Copy;
using System;
using System.Collections.Generic;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// click houser bulk copy extensions
    /// </summary>
    internal static class ClickHouseBulkCopyExtensions
    {
        /// <summary>
        /// 初始化赋值 # 反射赋值
        /// </summary>
        /// <param name="bulkCopy"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        internal static ClickHouseBulkCopy ReflectionSet(this ClickHouseBulkCopy bulkCopy, string destinationTableName, IReadOnlyCollection<string> columnNames)
        {
            if (null == bulkCopy)
                throw new ArgumentNullException(nameof(bulkCopy));

            var instanceT = bulkCopy.GetType();
            var pi_dest = instanceT.GetProperty(nameof(ClickHouseBulkCopy.DestinationTableName));
            pi_dest.SetValue(bulkCopy, destinationTableName);

            if (null != columnNames && columnNames.Count > 0)
            {
                var pi_cols = instanceT.GetProperty(nameof(ClickHouseBulkCopy.ColumnNames));
                pi_cols.SetValue(bulkCopy, columnNames);
            }

            return bulkCopy;
        }
    }
}
