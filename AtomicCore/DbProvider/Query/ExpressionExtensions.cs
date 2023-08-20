/*
 *  静态解析使用如下写法（老内核使用该方法）
 *  MethodCallExpression MergeExp = Expression.Call(
        typeof(Queryable).MakeGenericType(typeof(TSource)),
        "OrderByDescending",
        new Type[] { 
            typeof(TSource),
            typeof(TKey)
        },
        new Expression[] { 
            source.Body,
            keySelector
    });
    return Expression.Lambda<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>>(MergeExp, source.Parameters);
 * 
 *  实例解析使用如下写法（新内核,当前内核）
 *  MethodCallExpression MergeExp = Expression.Call(
        sqlExp.Body,
        "OrderByDescending",
        new Type[] {
            typeof(int)
        },
        new Expression[] {
            keySelector
    });
    return Expression.Lambda<Func<IDbFetchListQueryable<Member_UserBasics>, IDbFetchListQueryable<Member_UserBasics>>>(MergeExp, sqlExp.Parameters);
 */

using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 表达式静态拓展类
    /// </summary>
    public static class ExpressionExtensions
    {
        #region 原型拓展Expression<Func<M, bool>>

        /// <summary>
        /// 2个表达式进行非合并
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="one"></param>
        /// <returns></returns>
        public static Expression<Func<M, bool>> Not<M>(this Expression<Func<M, bool>> one)
        {
            var body = Expression.Not(one.Body);
            return Expression.Lambda<Func<M, bool>>(body, one.Parameters);
        }
        /// <summary>
        /// 2个标识进行与合并
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="one"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static Expression<Func<M, bool>> And<M>(this Expression<Func<M, bool>> one, Expression<Func<M, bool>> another)
        {
            var body = Expression.AndAlso(one.Body, another.Body);
            return Expression.Lambda<Func<M, bool>>(body, one.Parameters);
        }
        /// <summary>
        /// 2个表达式进行或合并
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="one"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static Expression<Func<M, bool>> Or<M>(this Expression<Func<M, bool>> one, Expression<Func<M, bool>> another)
        {
            var body = Expression.OrElse(one.Body, another.Body);
            return Expression.Lambda<Func<M, bool>>(body, one.Parameters);
        }

        #endregion

        #region 结合DBQueryable中的LambdaExpression基类拓展

        /// <summary>
        /// 正排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>> OrderBy<TSource, TKey>(this Expression<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderBy",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        /// <summary>
        /// 正排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>> OrderBy<TSource, TKey>(this Expression<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderBy",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        /// <summary>
        /// 正排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>> OrderBy<TSource, TKey>(this Expression<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderBy",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        /// <summary>
        /// 倒排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>> OrderByDescending<TSource, TKey>(this Expression<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderByDescending",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbFetchQueryable<TSource>, IDbFetchQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        /// <summary>
        /// 倒排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>> OrderByDescending<TSource, TKey>(this Expression<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderByDescending",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbFetchListQueryable<TSource>, IDbFetchListQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        /// <summary>
        /// 倒排序（多用于动态拼接排序）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Expression<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>> OrderByDescending<TSource, TKey>(this Expression<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>> source, Expression<Func<TSource, TKey>> keySelector)
            where TSource : IDbModel
        {
            if (null == source)
            {
                throw new Exception("source is null");
            }
            if (null == keySelector)
            {
                throw new Exception("keySelector is null");
            }

            MethodCallExpression MergeExp = Expression.Call(
                 source.Body,
                 "OrderByDescending",
                 new Type[] {
                    typeof(TKey)
                },
                 new Expression[] {
                    keySelector
                }
            );

            return Expression.Lambda<Func<IDbCalculateQueryable<TSource>, IDbCalculateQueryable<TSource>>>(MergeExp, source.Parameters);
        }

        #endregion
    }
}
