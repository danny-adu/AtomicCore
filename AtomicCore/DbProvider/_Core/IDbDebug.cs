using System.Data.Common;
using System.Text;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB调试接口
    /// </summary>
    public interface IDbDebug
    {
        /// <summary>
        /// 初始化Sql查询的Debug条件
        /// </summary>
        /// <param name="sqlBuilder">sql语句拼接对象</param>
        /// <param name="paramChar">sql的参数前缀符,例如:Mssql-@,Mysql-?</param>
        /// <param name="sqlParameters">参数集合</param>
        void DebugInit(StringBuilder sqlBuilder, char paramChar, params DbParameter[] sqlParameters);

        /// <summary>
        /// 获取需要被调试的Sql语句
        /// </summary>
        /// <returns></returns>
        string DebugSqlText { get; }

        /// <summary>
        /// 原始SQL语句（直接复制到查询分析器中可以被执行的）
        /// </summary>
        string OriginalSqlText { get; }

        /// <summary>
        /// 获取需要被调试的Sql参数
        /// </summary>
        /// <returns></returns>
        DbParameter[] DebugSqlParameters { get; }
    }
}
