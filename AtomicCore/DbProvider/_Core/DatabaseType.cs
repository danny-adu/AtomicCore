namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public static class DatabaseType
    {
        /// <summary>
        /// 微软SqlServer2005或更高版本的数据库
        /// </summary>
        public const string Mssql2008 = "Mssql2008";

        /// <summary>
        /// Mysql数据库
        /// </summary>
        public const string Mysql = "Mysql";

        /// <summary>
        /// SQLite
        /// </summary>
        public const string SQLite = "SQLite";
    }
}
