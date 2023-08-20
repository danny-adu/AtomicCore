using System;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 与数据库表中的列关联的列属性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbColumnAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DbColumnAttribute() { }

        /// <summary>
        /// 获取或设置数据源的列名称
        /// </summary>
        public string DbColumnName { get; set; }

        /// <summary>
        /// 获取或设置数据库列的类型。
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否包含数据库自动生成的值。
        /// </summary>
        public bool IsDbGenerated { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示该类成员是否表示作为表的整个主键或部分主键的列。
        /// </summary>
        public bool IsDbPrimaryKey { get; set; }

        /// <summary>
        /// 对应的映射的IDBModel实体属性名称(自动化映射的时候赋值,标签不需要赋值)
        /// </summary>
        public string PropertyNameMapping { get; set; }
    }
}
