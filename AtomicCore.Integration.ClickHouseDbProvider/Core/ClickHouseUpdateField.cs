using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse下需要被修改的属性项
    /// </summary>
    internal sealed class ClickHouseUpdateField
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClickHouseUpdateField() { }

        /// <summary>
        /// 需要被填充的Model成员信息
        /// </summary>
        public PropertyInfo PropertyItem { get; set; }

        /// <summary>
        /// 被指定修改的Sql片段语句
        /// </summary>
        public string UpdateTextFragment { get; set; }

        /// <summary>
        /// 使用的参数,若无参数允许为null
        /// </summary>
        public IEnumerable<ClickHouseParameterDesc> Parameter { get; set; }
    }
}
