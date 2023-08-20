using System.Linq.Expressions;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 标准查询提供者接口
    /// </summary>
    public interface IDbQueryProvider
    {
        /// <summary>
        /// 创建一个新的查询
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IDbQueryable<M> CreateQuery<M>(Expression expression) where M : IDbModel;
    }
}
