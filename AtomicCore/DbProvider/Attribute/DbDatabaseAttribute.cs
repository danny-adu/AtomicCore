using System;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 数据库名称元数据标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbDatabaseAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DbDatabaseAttribute()
        {

        }

        /// <summary>
        /// 数据库实例名称。
        /// </summary>
        public string Name { get; set; }
    }
}
