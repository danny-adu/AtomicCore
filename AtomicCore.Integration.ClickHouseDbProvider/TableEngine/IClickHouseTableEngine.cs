using AtomicCore.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Table Engine
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface IClickHouseTableEngine<M>
        where M : IDbModel, new()
    {
        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <returns></returns>
        DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys);

        /// <summary>
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <returns></returns>
        DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model);

        /// <summary>
        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
        /// </summary>
        /// <param name="taskList">任务数据</param>
        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
        /// <returns></returns>
        DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false);

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <returns></returns>
        DbNonRecord Delete(Expression<Func<M, bool>> deleteExp);
    }
}
