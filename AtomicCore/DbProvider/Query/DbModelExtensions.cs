using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB Model拓展方法
    /// </summary>
    public static class DbModelExtensions
    {
        /// <summary>
        /// 执行DB查询中的IN的操作
        /// </summary>
        /// <typeparam name="T">指定查询的接口类型中的属性的类型</typeparam>
        /// <param name="dbmodel">IDBModel接口实例</param>
        /// <param name="assignProperty">指定的字段</param>
        /// <param name="inCaseValue">IN查询值</param>
        /// <returns></returns>
        public static bool SqlIn<T>(this IDbModel dbmodel, T assignProperty, string inCaseValue)
        {
            return true;
        }

        /// <summary>
        /// 执行DB查询中的Not IN的操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbmodel">原型实例</param>
        /// <param name="assignProperty">指定属性,例如:d.name</param>
        /// <param name="inCaseValue">not in 的数据</param>
        /// <returns></returns>
        public static bool SqlNotIn<T>(this IDbModel dbmodel, T assignProperty, string inCaseValue)
        {
            return true;
        }

        /// <summary>
        /// where条件中嵌套子查询
        /// </summary>
        /// <typeparam name="TChild">子表类型</typeparam>
        /// <param name="dbmodel">原型实例</param>
        /// <param name="joinPredicate">子级联条件</param>
        /// <param name="childPredicate">子查询条件表达式</param>
        /// <param name="isExists">嵌套查询模式,True:Exists,False:Not Exists</param>
        /// <returns></returns>
        public static bool SqlSubQuery<TChild>(this IDbModel dbmodel, Expression<Func<TChild, bool>> joinPredicate, Expression<Func<TChild, bool>> childPredicate, bool isExists = true)
        {
            return true;
        }
    }
}
