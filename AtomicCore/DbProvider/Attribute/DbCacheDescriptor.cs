using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB的缓存信息描述（DB表与IDBModel模型的一一对应缓存）
    /// </summary>
    public class DbCacheDescriptor
    {
        /// <summary>
        /// DB数据库说明
        /// </summary>
        public DbDatabaseAttribute DbDatabase { get; set; }

        /// <summary>
        /// DB表自身说明
        /// </summary>
        public DbTableAttribute DbTable { get; set; }

        /// <summary>
        /// DB表的列(Key:模型的属性类型名称,Value:对应的数据库列信息)
        /// </summary>
        public Dictionary<PropertyInfo, DbColumnAttribute> DbColumns { get; set; }
    }
}
