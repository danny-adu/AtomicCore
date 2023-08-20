namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// Mssql参数定义描述
    /// </summary>
    internal class MssqlParameterDesc
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MssqlParameterDesc() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public MssqlParameterDesc(string key, object value, MssqlParameterDirection direction = MssqlParameterDirection.Input)
        {
            this.Name = key;
            this.Value = value;
            this.Direction = direction;
        }

        /// <summary>
        /// 构造函数(针对字符串的)
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        /// <param name="size">长度</param>
        public MssqlParameterDesc(string key, object value, MssqlParameterDirection direction, int size)
        {
            this.Name = key;
            this.Value = value;
            this.Direction = direction;
            this.Size = size;
        }

        /// <summary>
        /// 构造函数(针对Decimal的)
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        /// <param name="precision">数字类型的最大位数</param>
        /// <param name="scale">小数位</param>
        public MssqlParameterDesc(string key, object value, MssqlParameterDirection direction, byte precision, byte scale = byte.MinValue)
        {
            this.Name = key;
            this.Value = value;
            this.Direction = direction;
            this.Precision = precision;
            this.Scale = scale;
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
        /// 字段长度
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 解析的小数最大位数
        /// </summary>
        public byte Precision { get; set; }

        /// <summary>
        /// 解析的小数位
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public MssqlParameterDirection Direction { get; set; }
    }
}
