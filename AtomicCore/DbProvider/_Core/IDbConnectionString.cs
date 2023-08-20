namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 数据库访问链接字符串
    /// </summary>
    public interface IDbConnectionString
    {
        /// <summary>
        /// 获取指定类型所在的数据库连接字符串
        /// </summary>
        /// <returns></returns>
        string GetConnection();
    }

    /// <summary>
    /// 数据库访问链接字符串
    /// </summary>
    /// <typeparam name="DbModel">Db模型类型</typeparam>
    public interface IDbConnectionString<DbModel> : IDbConnectionString
        where DbModel : IDbModel
    {

    }
}
