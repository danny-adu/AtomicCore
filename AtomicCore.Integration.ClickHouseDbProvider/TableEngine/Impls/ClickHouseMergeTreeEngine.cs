//using AtomicCore.DbProvider;
//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace AtomicCore.Integration.ClickHouseDbProvider
//{
//    /// <summary>
//    /// ClickHouse MergeTree Engine
//    /// </summary>
//    public class ClickHouseMergeTreeEngine<M> : ClickHouseTableEngineBase, IClickHouseTableEngine<M>
//        where M : IDbModel, new()
//    {
//        /// <summary>
//        /// 更新操作（局部更新）
//        /// </summary>
//        /// <param name="whereExp">需要被更新的条件</param>
//        /// <param name="updatePropertys">需要被替换或更新的属性</param>
//        /// <returns></returns>
//        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 更新操作（整体更新）
//        /// </summary>
//        /// <param name="whereExp">需要被更新的条件</param>
//        /// <param name="model">需要被整体替换的实体</param>
//        /// <returns></returns>
//        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
//        /// </summary>
//        /// <param name="taskList">任务数据</param>
//        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
//        /// <returns></returns>
//        public DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 删除操作
//        /// </summary>
//        /// <param name="deleteExp">删除条件</param>
//        /// <returns></returns>
//        public DbNonRecord Delete(Expression<Func<M, bool>> deleteExp)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
