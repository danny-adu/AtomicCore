using System;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 将某个类指定为与数据库表相关联的实体类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DbTableAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DbTableAttribute() { }

        /// <summary>
        /// 获取或设置表或视图的名称
        /// </summary>
        public string Name { get; set; }
    }
}
