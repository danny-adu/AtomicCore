using System;
using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB按Model批量更新参数类
    /// </summary>
    public sealed class DbUpdateTaskData<M>
        where M : IDbModel, new()
    {
        /// <summary>
        /// 条件表达式
        /// </summary>
        public Expression<Func<M, bool>> WhereExp { get; set; }

        /// <summary>
        /// 更新属性表达式
        /// </summary>
        public Expression<Func<M, M>> UpdatePropertys { get; set; }
    }
}
