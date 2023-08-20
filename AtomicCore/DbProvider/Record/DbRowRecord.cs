using System.Collections.Generic;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB的行记录
    /// </summary>
    public class DbRowRecord : List<DbColumnRecord>
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbRowRecord()
        {
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取第一列的数据
        /// </summary>
        /// <returns></returns>
        public object GetDefaultValue()
        {
            if (this.Count > 0)
                return this[0].Value;
            
            return null;
        }

        #endregion
    }
}
