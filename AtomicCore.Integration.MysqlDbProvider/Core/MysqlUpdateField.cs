using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// SqlServer下需要被修改的属性项
    /// </summary>
    internal sealed class MysqlUpdateField
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MysqlUpdateField() { }

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
        public IEnumerable<MysqlParameterDesc> Parameter { get; set; }
    }
}
