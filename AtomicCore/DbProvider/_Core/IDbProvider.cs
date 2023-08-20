using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB数据源操作提供接口定义
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface IDbProvider<M>
        where M : IDbModel, new()
    {
        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="model">需要新增的数据实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbSingleRecord<M> Insert(M model, string suffix = null);

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="modelList">需要新增的数据实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbCollectionRecord<M> InsertBatch(IEnumerable<M> modelList, string suffix = null);

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys, string suffix = null);

        /// <summary>
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model, string suffix = null);

        /// <summary>
        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
        /// </summary>
        /// <param name="taskList">任务数据</param>
        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null);

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbNonRecord Delete(Expression<Func<M, bool>> deleteExp, string suffix = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbSingleRecord<M> Fetch(Expression<Func<IDbFetchQueryable<M>, IDbFetchQueryable<M>>> exp, string suffix = null);

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbCollectionRecord<M> FetchList(Expression<Func<IDbFetchListQueryable<M>, IDbFetchListQueryable<M>>> exp, string suffix = null);

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        DbCalculateRecord Calculate(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null);

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="model">需要新增的数据实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbSingleRecord<M>> InsertAsync(M model, string suffix = null);

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="modelList">需要新增的数据实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbCollectionRecord<M>> InsertBatchAsync(IEnumerable<M> modelList, string suffix = null);

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys, string suffix = null);

        /// <summary>
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model, string suffix = null);

        /// <summary>
        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
        /// </summary>
        /// <param name="taskList">任务数据</param>
        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbNonRecord> UpdateTaskAsync(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null);

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp, string suffix = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbSingleRecord<M>> FetchAsync(Expression<Func<IDbFetchQueryable<M>, IDbFetchQueryable<M>>> exp, string suffix = null);

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbCollectionRecord<M>> FetchListAsync(Expression<Func<IDbFetchListQueryable<M>, IDbFetchListQueryable<M>>> exp, string suffix = null);

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        Task<DbCalculateRecord> CalculateAsync(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null);
    }
}
