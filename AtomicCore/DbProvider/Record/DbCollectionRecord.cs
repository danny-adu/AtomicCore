using System.Collections.Generic;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 执行DB语句后返回多记录结果集
    /// </summary>
    /// <typeparam name="M">泛型:DB模式</typeparam>
    public sealed class DbCollectionRecord<M> : DbRecordBase
        where M : IDbModel
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbCollectionRecord() : base() { }

        #endregion

        #region Propertys

        private int _currentpage = 0;
        private int _pageSize = 0;
        private int _totalCount = 0;
        private List<M> _record = null;

        /// <summary>
        /// 当前的页码
        /// </summary>
        public int CurrentPage
        {
            get { return this._currentpage; }
            set { this._currentpage = value; }
        }

        /// <summary>
        /// 每页设置数据量
        /// </summary>
        public int PageSize
        {
            get { return this._pageSize; }
            set { this._pageSize = value; }
        }

        /// <summary>
        /// 符合条件的总数量
        /// </summary>
        public int TotalCount
        {
            get { return this._totalCount; }
            set { this._totalCount = value; }
        }

        /// <summary>
        /// 符合条件的列表
        /// </summary>
        public List<M> Record
        {
            get { return this._record; }
            set { this._record = value; }
        }

        #endregion
    }
}
