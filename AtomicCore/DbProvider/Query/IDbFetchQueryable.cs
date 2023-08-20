using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 标准的DB获取单个实体查询接口
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface IDbFetchQueryable<M>
        where M : IDbModel
    {
        /// <summary>
        /// 指定需要被查询的部分
        /// </summary>
        /// <param name="selector">需要被查询的字段</param>
        /// <returns></returns>
        IDbFetchQueryable<M> Select(Expression<Func<M, M>> selector);

        /// <summary>
        /// where查询条件部分
        /// </summary>
        /// <param name="predicate">where条件表达式</param>
        /// <returns></returns>
        IDbFetchQueryable<M> Where(Expression<Func<M, bool>> predicate);

        /// <summary>
        /// 正序列排序部分
        /// </summary>
        /// <typeparam name="TKey">M类型中指定排序依据的属性类型</typeparam>
        /// <param name="keySelector">M类型中指定排序依据的属性</param>
        /// <returns></returns>
        IDbFetchQueryable<M> OrderBy<TKey>(Expression<Func<M, TKey>> keySelector);

        /// <summary>
        /// 倒序排序部分
        /// </summary>
        /// <typeparam name="TKey">M类型中指定排序依据的属性类型</typeparam>
        /// <param name="keySelector">M类型中指定排序依据的属性</param>
        /// <returns></returns>
        IDbFetchQueryable<M> OrderByDescending<TKey>(Expression<Func<M, TKey>> keySelector);
    }
}
