using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Mysql参数定义描述
    /// </summary>
    internal class MysqlParameterDesc
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MysqlParameterDesc() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public MysqlParameterDesc(string key, object value, MysqlParameterDirection direction = MysqlParameterDirection.Input)
        {
            this.Name = key;
            this.Value = value;
            this.Direction = direction;
        }

        /// <summary>
        /// Name名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public MysqlParameterDirection Direction { get; set; }
    }
}
