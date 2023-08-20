using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 标准查询接口定义
    /// </summary>
    /// <typeparam name="M">必须是继承IModel的实体</typeparam>
    public interface IDbQueryable<out M>
        where M : IDbModel
    {
        /// <summary>
        /// 当前的表达式
        /// </summary>
        Expression Expression { get; }

        /// <summary>
        /// 查询提供者
        /// </summary>
        IDbQueryProvider Provider { get; }
    }
}
