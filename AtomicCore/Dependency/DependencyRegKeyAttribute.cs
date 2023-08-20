using System;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// 自动化依赖注册别名元数据标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DependencyRegKeyAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public DependencyRegKeyAttribute(string name)
            : base()
        {
            this.Name = name;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
    }
}
