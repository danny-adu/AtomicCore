using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 返回DB计算结果集
    /// </summary>
    public sealed class DbCalculateRecord : DbRecordBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbCalculateRecord()
            : base()
        {

        }

        #endregion

        #region Propertys

        private IEnumerable<DbRowRecord> _record = null;

        /// <summary>
        /// 计算的结果集
        /// </summary>
        public IEnumerable<DbRowRecord> Record
        {
            get { return this._record; }
            set { this._record = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 返回第一行第一列数据
        /// </summary>
        /// <returns></returns>
        public object GetDefaultValue()
        {
            if (null != this._record && this._record.Count() > 0)
                return this._record.First().GetDefaultValue();
            
            return null;
        }

        /// <summary>
        /// 返回第一行第一列数据
        /// </summary>
        /// <returns></returns>
        public T GetDefaultValue<T>()
            where T : IConvertible
        {
            T result = default;
            if (this._record != null && this._record.Count() > 0)
            {
                object obj = this._record.First().GetDefaultValue();
                if (null == obj)
                    result = default;
                else
                    result = (T)Convert.ChangeType(obj, typeof(T));
            }

            return result;
        }

        #endregion
    }
}
