using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 标准的Sql获取集合查询接口
    /// </summary>
    /// <typeparam name="M">数据模型实体类型</typeparam>
    public interface IDbFetchListQueryable<M>
        where M : IDbModel
    {
        /// <summary>
        /// 指定需要被查询的部分
        /// </summary>
        /// <param name="selector">筛选表达式</param>
        /// <returns></returns>
        IDbFetchListQueryable<M> Select(Expression<Func<M, M>> selector);

        /// <summary>
        /// where查询条件部分
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        IDbFetchListQueryable<M> Where(Expression<Func<M, bool>> predicate);

        /// <summary>
        /// 正序列排序部分
        /// </summary>
        /// <typeparam name="TKey">M类型中指定排序依据的属性类型</typeparam>
        /// <param name="keySelector">排序表达式</param>
        /// <returns></returns>
        IDbFetchListQueryable<M> OrderBy<TKey>(Expression<Func<M, TKey>> keySelector);

        /// <summary>
        /// 倒序排序部分
        /// </summary>
        /// <typeparam name="TKey">M类型中指定排序依据的属性类型</typeparam>
        /// <param name="keySelector">排序表达式</param>
        /// <returns></returns>
        IDbFetchListQueryable<M> OrderByDescending<TKey>(Expression<Func<M, TKey>> keySelector);

        /// <summary>
        /// Sql语句中指定翻页部分
        /// </summary>
        /// <param name="currentPage">第几页</param>
        /// <param name="pageSize">每页多少条</param>
        /// <returns></returns>
        IDbFetchListQueryable<M> Pager(int currentPage, int pageSize);
    }
}
