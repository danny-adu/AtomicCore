namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// SqlServer下Model属性与查询使用的字段的映射关系对象
    /// </summary>
    internal sealed class MssqlSelectField
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MssqlSelectField()
        {
            this.IsModelProperty = false;
        }

        /// <summary>
        /// 查询的字段或表达式的片段语句，例如：name 或 count(1) as num中的count(1)
        /// </summary>
        public string DBSelectFragment { get; set; }

        /// <summary>
        /// 该属性为查询中出现的字段名称，也可以用于dataread中指定获取数据列,例如：name as n 或 count(1) as num中as后的定义
        /// </summary>
        public string DBFieldAsName { get; set; }

        /// <summary>
        /// 是否与生成的Model中的属性有对应
        /// </summary>
        public bool IsModelProperty { get; set; }
    }
}
