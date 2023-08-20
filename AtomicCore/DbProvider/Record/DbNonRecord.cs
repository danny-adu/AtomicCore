namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 执行DB语句后返回无记录结果集
    /// </summary>
    public sealed class DbNonRecord : DbRecordBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbNonRecord() : base() { }

        #endregion

        #region Propertys

        /// <summary>
        /// 受影响行数
        /// </summary>
        public int AffectedRow { get; set; } = 0;

        #endregion
    }
}
