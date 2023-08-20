namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB的列记录
    /// </summary>
    public class DbColumnRecord
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbColumnRecord() { }

        #endregion

        #region Propertys

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列值
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}
