namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 执行DB语句后返回单记录结果集
    /// </summary>
    /// <typeparam name="M">泛型:DB模式</typeparam>
    public sealed class DbSingleRecord<M> : DbRecordBase
        where M : IDbModel
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbSingleRecord()
            : base()
        {

        }

        #endregion

        #region Propertys

        private M _record = default;

        /// <summary>
        /// 单记录结果
        /// </summary>
        public M Record
        {
            get { return this._record; }
            set { this._record = value; }
        }

        #endregion
    }
}
