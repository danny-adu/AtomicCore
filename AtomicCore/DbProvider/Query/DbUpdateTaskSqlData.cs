using System.Collections.Generic;
using System.Data.Common;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 批量更新任务Sql执行数据
    /// </summary>
    public class DbUpdateTaskSqlData
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        public string SqlText { get; set; }

        /// <summary>
        /// 执行参数
        /// </summary>
        public DbParameter[] SqlParameters { get; set; }
    }
}
