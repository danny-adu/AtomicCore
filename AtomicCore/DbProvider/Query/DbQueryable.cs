namespace AtomicCore.DbProvider
{
    #region Static Extensions

    /// <summary>
    /// 基于IDBFetchQueryable IDBFetchListQueryable IDBCalculateQueryable的原型静态拓展
    /// </summary>
    public static class DbQueryableExtensions
    {
        #region IDbFetchQueryable

        ///// <summary>
        ///// 指定需要被查询的部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="selector"></param>
        ///// <returns></returns>
        //public static IDBFetchQueryable<TResult> Select<TSource, TResult>(this IDBFetchQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        //    where TSource : IDBModel
        //    where TResult : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (selector == null)
        //    {
        //        throw new Exception("selector");
        //    }
        //    return (IDBFetchQueryable<TResult>)source.Provider.CreateQuery<TResult>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
        //}

        ///// <summary>
        ///// where查询条件部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public static IDBFetchQueryable<TSource> Where<TSource>(this IDBFetchQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (predicate == null)
        //    {
        //        throw new Exception("predicate");
        //    }
        //    return (IDBFetchQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(predicate) }));
        //}

        ///// <summary>
        ///// 正序列排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBFetchQueryable<TSource> OrderBy<TSource, TKey>(this IDBFetchQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBFetchQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// 倒序排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBFetchQueryable<TSource> OrderByDescending<TSource, TKey>(this IDBFetchQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBFetchQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        #endregion

        #region IDbFetchListQueryable

        ///// <summary>
        ///// 指定需要被查询的部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="selector"></param>
        ///// <returns></returns>
        //public static IDBFetchListQueryable<TResult> Select<TSource, TResult>(this IDBFetchListQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        //    where TSource : IDBModel
        //    where TResult : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (selector == null)
        //    {
        //        throw new Exception("selector");
        //    }
        //    return (IDBFetchListQueryable<TResult>)source.Provider.CreateQuery<TResult>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
        //}

        ///// <summary>
        ///// where查询条件部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public static IDBFetchListQueryable<TSource> Where<TSource>(this IDBFetchListQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (predicate == null)
        //    {
        //        throw new Exception("predicate");
        //    }
        //    return (IDBFetchListQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(predicate) }));
        //}

        ///// <summary>
        ///// 正序列排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBFetchListQueryable<TSource> OrderBy<TSource, TKey>(this IDBFetchListQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBFetchListQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// 倒序排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBFetchListQueryable<TSource> OrderByDescending<TSource, TKey>(this IDBFetchListQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBFetchListQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// Sql语句中指定翻页部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="currentPage">当前页面索引</param>
        ///// <param name="pageSize">每页多少数据</param>
        ///// <returns></returns>
        //public static IDBFetchListQueryable<TSource> Pager<TSource>(this IDBFetchListQueryable<TSource> source, int currentPage, int pageSize)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    return (IDBFetchListQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Constant(currentPage), Expression.Constant(pageSize) }));
        //}

        #endregion

        #region IDbCalculateQueryable

        ///// <summary>
        ///// 指定需要被查询的部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="selector"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TResult> Select<TSource, TResult>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        //    where TSource : IDBModel
        //    where TResult : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (selector == null)
        //    {
        //        throw new Exception("selector");
        //    }
        //    return (IDBCalculateQueryable<TResult>)source.Provider.CreateQuery<TResult>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), new Expression[] { source.Expression, Expression.Quote(selector) }));
        //}

        ///// <summary>
        ///// Sql语句中的Count(x)部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <param name="alias"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> Count<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, string alias)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// Sql语句中的Sum(x)部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <param name="alias"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> Sum<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, string alias)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// Sql语句中的MAX(x)部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <param name="alias"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> Max<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, string alias)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// Sql语句中的MIN(x)部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <param name="alias"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> Min<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, string alias)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// where查询条件部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> Where<TSource>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (predicate == null)
        //    {
        //        throw new Exception("predicate");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression, Expression.Quote(predicate) }));
        //}

        ///// <summary>
        ///// 正序列排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> OrderBy<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// 倒序排序部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> OrderByDescending<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        ///// <summary>
        ///// Sql语句中的GroupBy部分
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="keySelector"></param>
        ///// <returns></returns>
        //public static IDBCalculateQueryable<TSource> GroupBy<TSource, TKey>(this IDBCalculateQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        //    where TSource : IDBModel
        //{
        //    if (source == null)
        //    {
        //        throw new Exception("source");
        //    }
        //    if (keySelector == null)
        //    {
        //        throw new Exception("keySelector");
        //    }
        //    return (IDBCalculateQueryable<TSource>)source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), new Expression[] { source.Expression, Expression.Quote(keySelector) }));
        //}

        #endregion
    }

    #endregion
}
