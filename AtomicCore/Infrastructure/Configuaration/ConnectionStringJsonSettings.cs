namespace AtomicCore
{
    /// <summary>
    /// 数据库链接字符串Json Settings
    /// </summary>
    public class ConnectionStringJsonSettings
    {
        #region Propertys

        /// <summary>
        /// 数据库链接名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 引擎提供者名称
        /// </summary>
        public string ProviderName { get; set; }

        #endregion
    }
}
