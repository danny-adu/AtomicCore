using System.Threading.Tasks;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 数据存储过程接口
    /// </summary>
    public interface IDbProcedurer
    {
        /// <summary>
        /// 在当前数据源下执行脚本或命令
        /// </summary>
        /// <param name="inputData">输入的参数对象</param>
        /// <returns></returns>
        DbProcedureRecord Execute(DbExecuteInputBase inputData);

        /// <summary>
        /// 在当前数据源下执行脚本或命令 - 异步
        /// </summary>
        /// <param name="inputData">输入的参数对象</param>
        /// <returns></returns>
        Task<DbProcedureRecord> ExecuteAsync(DbExecuteInputBase inputData);
    }
}
