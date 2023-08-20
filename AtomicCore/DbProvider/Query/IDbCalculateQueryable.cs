using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 标准的Sql统计计算查询接口
    /// </summary>
    /// <typeparam name="M">数据模型类型</typeparam>
    public interface IDbCalculateQueryable<M>
        where M : IDbModel
    {
        /// <summary>
        /// 指定需要被查询的部分
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Select(Expression<Func<M, M>> selector);

        /// <summary>
        /// Sql语句中的Count(x)部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Count<TKey>(Expression<Func<M, TKey>> keySelector, string alias);

        /// <summary>
        /// Sql语句中的Sum(x)部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Sum<TKey>(Expression<Func<M, TKey>> keySelector, string alias);

        /// <summary>
        /// Sql语句中的MAX(x)部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Max<TKey>(Expression<Func<M, TKey>> keySelector, string alias);

        /// <summary>
        /// Sql语句中的MIN(x)部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Min<TKey>(Expression<Func<M, TKey>> keySelector, string alias);

        /// <summary>
        /// where查询条件部分
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> Where(Expression<Func<M, bool>> predicate);

        /// <summary>
        /// 正序列排序部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> OrderBy<TKey>(Expression<Func<M, TKey>> keySelector);

        /// <summary>
        /// 倒序排序部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> OrderByDescending<TKey>(Expression<Func<M, TKey>> keySelector);

        /// <summary>
        /// Sql语句中的GroupBy部分
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IDbCalculateQueryable<M> GroupBy<TKey>(Expression<Func<M, TKey>> keySelector);
    }
}
